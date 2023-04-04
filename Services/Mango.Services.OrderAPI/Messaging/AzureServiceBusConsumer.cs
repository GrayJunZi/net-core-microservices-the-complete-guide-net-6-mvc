using Azure.Messaging.ServiceBus;
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
    private readonly IOrderRepository _orderRepository;
    private readonly IConfiguration _configuration;

    private ServiceBusProcessor checkOutPricessor;

    public AzureServiceBusConsumer(IOrderRepository orderRepository, IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _configuration = configuration;

        _serviceBusConnectionString = _configuration["ServiceBusConnectionString"];
        _serviceBusConnectionString = _configuration["CheckoutMessageTopic"];
        _serviceBusConnectionString = _configuration["SubscriptionsName"];

        var client = new ServiceBusClient(_serviceBusConnectionString);

        checkOutPricessor = client.CreateProcessor(_checkoutMessageTopic, _subscriptionsName);
    }

    public async Task Start()
    {
        checkOutPricessor.ProcessMessageAsync += OnCheckoutMessageReceived;
        checkOutPricessor.ProcessErrorAsync += ErrorHandler;
        await checkOutPricessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await checkOutPricessor.StopProcessingAsync();
        await checkOutPricessor.DisposeAsync();
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

        var checkoutHeaderDto = JSONHelper.Deserialize<CheckoutHeaderDto>(body);

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
    }
}


public static class JSONHelper
{
    public static T Deserialize<T>(object json)
    {
        return JsonSerializer.Deserialize<T>(json.ToString(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
    }
}