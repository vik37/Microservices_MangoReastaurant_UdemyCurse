using Azure.Messaging.ServiceBus;
using System.Text;
using Mango.Services.Email.Repository;
using Newtonsoft.Json;
using Mango.Services.Email.Messages;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _subscriptionEmail;
        private readonly string _orderUpdatePaymentResultTopic;

        private ServiceBusProcessor _processorForOrderUpdatePayment;

        private readonly IConfiguration _config;

        private readonly EmailRepository _emailRepository;
        public AzureServiceBusConsumer(EmailRepository emailRepository, IConfiguration config)
        {
            _config = config;
            _emailRepository = emailRepository;
            _serviceBusConnectionString = _config.GetValue<string>("ServiceBusConnectionString");
            _subscriptionEmail = _config.GetValue<string>("SubscriptionName");
            _orderUpdatePaymentResultTopic = _config.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _processorForOrderUpdatePayment = client.CreateProcessor(_orderUpdatePaymentResultTopic,_subscriptionEmail);
        }
        public async Task Start()
        {
            _processorForOrderUpdatePayment.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            _processorForOrderUpdatePayment.ProcessErrorAsync += ErrorHandler;
            await _processorForOrderUpdatePayment.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _processorForOrderUpdatePayment.StopProcessingAsync();
            await _processorForOrderUpdatePayment.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage objMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            try
            {
                await _emailRepository.SendAndLogEmail(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
