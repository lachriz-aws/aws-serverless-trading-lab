using Amazon.DynamoDBv2;
using Amazon.SimpleNotificationService;
using Amazon.StepFunctions;
using Microsoft.Extensions.DependencyInjection;
using ServerlessTrading.Lib.Services;

namespace ServerlessTrading.Lib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTradeService(this IServiceCollection services, Action<TradeServiceOptions> configure)
        {
            // Service registration
            services.AddSingleton<TradeService>();
            services.Configure(configure);

            // AWS Services
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonStepFunctions>();

            // Core services
            services.AddCoreServices();

            return services;
        }

        public static IServiceCollection AddNotificationService(this IServiceCollection services, Action<NotificationServiceOptions> configure)
        {
            // Service registration
            services.AddSingleton<NotificationService>();
            services.Configure(configure);

            // AWS Services
            services.AddAWSService<IAmazonSimpleNotificationService>();

            // Core services
            services.AddCoreServices();

            return services;
        }

        private static void AddCoreServices(this IServiceCollection services)
        {
            // Logging
            services.AddLogging();
        }
    }
}
