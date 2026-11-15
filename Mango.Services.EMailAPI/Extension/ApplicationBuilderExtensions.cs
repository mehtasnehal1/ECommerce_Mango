using Mango.Services.EMailAPI.Messaging;
using System.Reflection.Metadata;

namespace Mango.Services.EMailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostapplicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostapplicationLifetime.ApplicationStarted.Register(OnStart);
            hostapplicationLifetime.ApplicationStopping.Register(OnStop);

            return app;
        }

        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }

        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }
    }
}
