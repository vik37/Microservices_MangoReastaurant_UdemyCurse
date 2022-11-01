using Azure.Messaging.ServiceBus;
using Mango.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Mango.MessageBus;
using PaymentProcessor;
using Mango.Services.Payment.Messages;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _subscriptionPayment;
        private readonly string _orderPaymentProccessTopic;
        private readonly string _orderUpdatePaymentResultTopic;

        private readonly IMessageBus _messageBus;
        private ServiceBusProcessor _orderPaymentProcessor;
        private readonly IConfiguration _config;
        private readonly IProcessPayment _processPayment;

        public AzureServiceBusConsumer( IConfiguration config, IProcessPayment processPayment,
                                        IMessageBus messageBus)
        {
            _config = config;
            _messageBus = messageBus;
            _processPayment = processPayment;
            _serviceBusConnectionString = _config.GetValue<string>("ServiceBusConnectionString");
            _subscriptionPayment = _config.GetValue<string>("OrderPaymentProcessSubscription");
            _orderPaymentProccessTopic = _config.GetValue<string>("OrderPaymentProccessTopic");

            _orderUpdatePaymentResultTopic = _config.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _orderPaymentProcessor = client.CreateProcessor(_orderPaymentProccessTopic, _subscriptionPayment);
        }
        public async Task Start()
        {
            _orderPaymentProcessor.ProcessMessageAsync += ProcessPayments;
            _orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderPaymentProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _orderPaymentProcessor.StopProcessingAsync();
            await _orderPaymentProcessor.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task ProcessPayments(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email = paymentRequestMessage.Email
            };
            

            try
            {
                await _messageBus.PublishMessage(updatePaymentResultMessage, _orderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
