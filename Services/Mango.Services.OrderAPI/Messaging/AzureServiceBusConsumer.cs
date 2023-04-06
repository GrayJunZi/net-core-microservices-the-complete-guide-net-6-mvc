using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Common;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using System.Text;
using System.Text.Json;

namespace Mango.Services.OrderAPI.Messaging;
public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _checkoutMessageTopic;
    private readonly string _subscriptionsName;
    private readonly string _orderPaymentProcessTopic;
    private readonly string _orderUpdatePaymentResultTopic;
    private readonly IOrderRepository _orderRepository;
    private readonly IMessageBus _messageBus;
    private readonly IConfiguration _configuration;

    private ServiceBusProcessor checkOutProcessor;
    private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

    public AzureServiceBusConsumer(
        IOrderRepository orderRepository,
        IMessageBus messageBus,
        IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _messageBus = messageBus;
        _configuration = configuration;

        _serviceBusConnectionString = _configuration["ServiceBusConnectionString"];
        _checkoutMessageTopic = _configuration["CheckoutMessageTopic"];
        _subscriptionsName = _configuration["SubscriptionsName"];
        _orderPaymentProcessTopic = _configuration["OrderPaymentProcessTopic"];
        _orderUpdatePaymentResultTopic = _configuration["OrderUpdatePaymentResultTopic"];

        var client = new ServiceBusClient(_serviceBusConnectionString);

        checkOutProcessor = client.CreateProcessor(_checkoutMessageTopic, _subscriptionsName);
        orderUpdatePaymentStatusProcessor = client.CreateProcessor(_orderUpdatePaymentResultTopic, _subscriptionsName);
    }

    public async Task Start()
    {
        checkOutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
        checkOutProcessor.ProcessErrorAsync += ErrorHandler;
        await checkOutProcessor.StartProcessingAsync();

        orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await checkOutProcessor.StopProcessingAsync();
        await checkOutProcessor.DisposeAsync();

        await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        await orderUpdatePaymentStatusProcessor.DisposeAsync();
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

        var checkoutHeaderDto = JsonHelper.Deserialize<CheckoutHeaderDto>(body);

        var orderHeader = new OrderHeader
        {
            UserId = checkoutHeaderDto.UserId,
            Name = checkoutHeaderDto.Name,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = checkoutHeaderDto.CardNumber,
            CouponCode = checkoutHeaderDto.CouponCode,
            CVV = checkoutHeaderDto.CVV,
            DiscountTotal = checkoutHeaderDto.DiscountTotal,
            Email = checkoutHeaderDto.Email,
            ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
            OrderTime = DateTime.Now,
            OrderTotal = checkoutHeaderDto.OrderTotal,
            PaymentStatus = false,
            Phone = checkoutHeaderDto.Phone,
            PickupDateTime = checkoutHeaderDto.PickupDateTime,
        };

        foreach (var cart in checkoutHeaderDto.CartDetails)
        {
            var orderDetail = new OrderDetail
            {
                ProductId = cart.ProductId,
                ProductName = cart.Product.Name,
                Price = cart.Product.Price,
                Count = cart.Count,
            };
            orderHeader.CartTotalItems += cart.Count;
            orderHeader.OrderDetails.Add(orderDetail);
        }

        await _orderRepository.AddOrder(orderHeader);

        var paymentRequestMessage = new PaymentRequestMessage
        {
            Name = orderHeader.Name,
            CardNumber = orderHeader.CardNumber,
            CVV = orderHeader.CVV,
            ExpiryMonthYear = orderHeader.ExpiryMonthYear,
            OrderId = orderHeader.Id,
            OrderTotal = orderHeader.OrderTotal,
        };

        try
        {
            await _messageBus.PublishMessage(paymentRequestMessage, _orderPaymentProcessTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        var paymentResultMessage = JsonHelper.Deserialize<UpdatePaymentResultMessage>(body);

        await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
        await args.CompleteMessageAsync(args.Message);
    }
}