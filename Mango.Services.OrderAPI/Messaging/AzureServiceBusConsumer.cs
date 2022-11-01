using Azure.Messaging.ServiceBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Mango.MessageBus;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _subscriptionCheckout;
        private readonly string _checkoutMessageTopic;
        private readonly string _orderUpdatePaymentResultTopic;

        private readonly string _orderPaymentProccessTopic;

        private readonly IMessageBus _messageBus;

        private ServiceBusProcessor _processorForCheckout;
        private ServiceBusProcessor _processorForOrderUpdatePayment;

        private readonly IConfiguration _config;

        private readonly OrderRepository _orderRepository;
        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration config,
                                        IMessageBus messageBus)
        {
            _config = config;
            _orderRepository = orderRepository;
            _messageBus = messageBus;
            _serviceBusConnectionString = _config.GetValue<string>("ServiceBusConnectionString");
            _checkoutMessageTopic = _config.GetValue<string>("CheckoutMessageTopic");
            _subscriptionCheckout = _config.GetValue<string>("SubscriptionCheckout");
            _orderPaymentProccessTopic = _config.GetValue<string>("OrderPaymentProccessTopic");
            _orderUpdatePaymentResultTopic = _config.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _processorForCheckout = client.CreateProcessor(_checkoutMessageTopic);
            _processorForOrderUpdatePayment = client.CreateProcessor(_orderUpdatePaymentResultTopic, _subscriptionCheckout);
        }
        public async Task Start()
        {
            _processorForCheckout.ProcessMessageAsync += OnCheckOutMessageReceived;
            _processorForCheckout.ProcessErrorAsync += ErrorHandler;
            await _processorForCheckout.StartProcessingAsync();

            _processorForOrderUpdatePayment.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            _processorForOrderUpdatePayment.ProcessErrorAsync += ErrorHandler;
            await _processorForOrderUpdatePayment.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _processorForCheckout.StopProcessingAsync();
            await _processorForCheckout.DisposeAsync();

            await _processorForOrderUpdatePayment.StopProcessingAsync();
            await _processorForOrderUpdatePayment.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpireMonthlyYear = checkoutHeaderDto.ExpireMonthlyYear,
                OrderTime = DateTime.Now,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                PaymentStatus = false,
                Phone = checkoutHeaderDto.Phone,
                PickupDateTime = checkoutHeaderDto.PickupDateTime
            };
            foreach (var detaillist in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = detaillist.ProductId,
                    ProductName = detaillist.Product.Name,
                    Price = detaillist.Product.Price,
                    Count = detaillist.Count
                };
                orderHeader.CartTotalItems += detaillist.Count;               
            }
            await _orderRepository.AddOrder(orderHeader);

            PaymentRequestMessage paymentRequestMessage = new()
            {
                Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpireMonthlYear = orderHeader.ExpireMonthlyYear,
                OrderId = orderHeader.OrderHeaderId,
                OrderTotal = orderHeader.OrderTotal,
                Email = orderHeader.Email
            };

            try
            {
                await _messageBus.PublishMessage(paymentRequestMessage, _orderPaymentProccessTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);

            await args.CompleteMessageAsync(args.Message);
        }
    }
}
