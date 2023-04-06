using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.PaymentProcessor;
using Mango.Services.PaymentAPI.Common;
using Mango.Services.PaymentAPI.Messages;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging;
public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _orderPaymentProcessTopic;
    private readonly string _orderPaymentProcessSubscription;
    private readonly string _orderUpdatePaymentResultTopic;

    private readonly IProcessPayment _processPayment;
    private readonly IMessageBus _messageBus;
    private readonly IConfiguration _configuration;

    private ServiceBusProcessor orderPaymentProcessor;

    public AzureServiceBusConsumer(
        IProcessPayment processPayment,
        IMessageBus messageBus,
        IConfiguration configuration)
    {
        _processPayment = processPayment;
        _messageBus = messageBus;
        _configuration = configuration;

        _serviceBusConnectionString = _configuration["ServiceBusConnectionString"];
        _orderPaymentProcessTopic = _configuration["OrderPaymentProcessTopic"];
        _orderPaymentProcessSubscription = _configuration["OrderPaymentProcessSubscription"];
        _orderUpdatePaymentResultTopic = _configuration["OrderUpdatePaymentResultTopic"];

        var client = new ServiceBusClient(_serviceBusConnectionString);

        orderPaymentProcessor = client.CreateProcessor(_orderPaymentProcessTopic, _orderPaymentProcessSubscription);
    }

    public async Task Start()
    {
        orderPaymentProcessor.ProcessMessageAsync += OnProcessPayment;
        orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
        await orderPaymentProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await orderPaymentProcessor.StopProcessingAsync();
        await orderPaymentProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnProcessPayment(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        var paymentRequestMessage = JsonHelper.Deserialize<PaymentRequestMessage>(body);

        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage updatePaymentResultMessage = new()
        {
            Status = result,
            OrderId = paymentRequestMessage.OrderId,
        };

        try
        {
            await _messageBus.PublishMessage(updatePaymentResultMessage, _orderUpdatePaymentResultTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}