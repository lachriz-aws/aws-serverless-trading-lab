using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ServerlessTrading.Functions.Handlers;
using ServerlessTrading.Functions.Models;
using ServerlessTrading.Lib.Extensions;

namespace ServerlessTrading.Functions
{
    public static class Handler
    {
        private static readonly IServiceProvider ServiceProvider;

        static Handler()
        {
            var services = new ServiceCollection();

            // Handlers
            services.AddSingleton<CreateSampleTradesHandler>();
            services.AddSingleton<ManualTradeApprovalHandler>();

            // Services
            services.AddTradeService(options =>
            {
                options.TradesTableName = Environment.GetEnvironmentVariable("ServerlessTrading__Config__TradesTableName");
            });
            services.AddNotificationService(options =>
            {
                options.SnsNotificationTopicArn = Environment.GetEnvironmentVariable("ServerlessTrading__Config__SnsNotificationTopicArn");
                options.TradingApiBaseUrl = Environment.GetEnvironmentVariable("ServerlessTrading__Config__TradingApiBaseUrl");
            });

            // Logging
            services.AddLogging(builder => builder.AddSimpleConsole(options => options.ColorBehavior = LoggerColorBehavior.Disabled));

            // Build service provider
            ServiceProvider = services.BuildServiceProvider();
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
        public static async Task<CreateSampleTradesResponse> CreateSampleTradesAsync(CreateSampleTradesRequest request, ILambdaContext _)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<CreateSampleTradesHandler>();
                return await handler.RunAsync(request);
            }
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
        public static async Task StartManualTradeApprovalAsync(TaskToken<RequestTradeApprovalMessage> request, ILambdaContext _)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<ManualTradeApprovalHandler>();
                await handler.RunAsync(request);
            }
        }
    }
}