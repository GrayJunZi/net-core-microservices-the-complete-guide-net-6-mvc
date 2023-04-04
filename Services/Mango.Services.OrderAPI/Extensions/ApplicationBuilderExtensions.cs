using Mango.Services.OrderAPI.Messaging;

namespace Mango.Services.OrderAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        var hostApplicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLifetime.ApplicationStarted.Register(OnStart);
        hostApplicationLifetime.ApplicationStopped.Register(OnStop);

        return app;
    }

    private static void OnStart()
    {
        ServiceBusConsumer.Start();
    }

    private static void OnStop()
    {
        ServiceBusConsumer.Start();
    }
}
