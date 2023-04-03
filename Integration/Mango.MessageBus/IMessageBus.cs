using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;

namespace Mango.MessageBus;

public interface IMessageBus
{
    Task PublishMessage(BaseMessage message, string topicName);
}

public class AzureServiceBusMessageBus : IMessageBus
{
    private string connectionString = "";
    public async Task PublishMessage(BaseMessage message, string topicName)
    {
        await using var client = new ServiceBusClient(connectionString);

        var sender = client.CreateSender(topicName);

        var jsonMessage = JsonSerializer.Serialize(message);
        var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(finalMessage);

        await client.CloseAsync();
    }
}