using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.Email.Common;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using System.Text;

namespace Mango.Services.Email.Messaging;
public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _subscriptionsName;
    private readonly string _orderUpdatePaymentResultTopic;

    private readonly IEmailRepository _emailRepository;
    private readonly IMessageBus _messageBus;
    private readonly IConfiguration _configuration;

    private ServiceBusProcessor emailProcessor;

    public AzureServiceBusConsumer(
        IEmailRepository emailRepository,
        IMessageBus messageBus,
        IConfiguration configuration)
    {
        _emailRepository = emailRepository;
        _messageBus = messageBus;
        _configuration = configuration;

        _serviceBusConnectionString = _configuration["ServiceBusConnectionString"];
        _subscriptionsName = _configuration["SubscriptionsName"];
        _orderUpdatePaymentResultTopic = _configuration["OrderUpdatePaymentResultTopic"];

        var client = new ServiceBusClient(_serviceBusConnectionString);

        emailProcessor = client.CreateProcessor(_orderUpdatePaymentResultTopic, _subscriptionsName);
    }

    public async Task Start()
    {
        emailProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
        emailProcessor.ProcessErrorAsync += ErrorHandler;
        await emailProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await emailProcessor.StopProcessingAsync();
        await emailProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        var updatePaymentResultMessage = JsonHelper.Deserialize<UpdatePaymentResultMessage>(body);

        try
        {
            await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}