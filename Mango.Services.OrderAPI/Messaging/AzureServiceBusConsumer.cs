using Azure.Messaging.ServiceBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _subscriptionNameForCheckout;
        private readonly string _checkoutMessageTopic;

        private ServiceBusProcessor _processor;

        private readonly IConfiguration _config;

        private readonly OrderRepository _orderRepository;
        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration config)
        {
            _config = config;
            _orderRepository = orderRepository;
            _serviceBusConnectionString = _config.GetValue<string>("ServiceBusConnectionString");
            _checkoutMessageTopic = _config.GetValue<string>("CheckoutMessageTopic");
            _subscriptionNameForCheckout = _config.GetValue<string>("SubscriptionCheckout");

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _processor = client.CreateProcessor(_checkoutMessageTopic, _subscriptionNameForCheckout);
        }
        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnCheckOutMessageReceived;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
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
                await _orderRepository.AddOrder(orderHeader);
            }
        }
    }
}
