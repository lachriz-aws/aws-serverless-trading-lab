using Microsoft.Extensions.Logging.Console;
using ServerlessTrading.Lib.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddTradeService(options =>
{
    options.TradesTableName = builder.Configuration["ServerlessTrading:Config:TradesTableName"];
});

// Logging
builder.Services.AddLogging(x => x.AddSimpleConsole(y => y.ColorBehavior = LoggerColorBehavior.Disabled));

// Build app
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UsePathBase(builder.Configuration["ServerlessTrading:Hosting:PathBase"] ?? string.Empty);
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
