using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var meter = new Meter("DotnetGrafanaMonitoringApp.CustomMetrics", "1.0");

var customCounter = meter.CreateCounter<long>("custom_http_requests_total",
    description: "The total count of custom HTTP requests.");

builder.Services.AddOpenTelemetry()
    .WithMetrics(opt =>
    {
        opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("DotnetGrafanaMonitoringApp"))
           .AddMeter("DotnetGrafanaMonitoringApp.CustomMetrics")
           .AddAspNetCoreInstrumentation()
           .AddRuntimeInstrumentation()
           .AddProcessInstrumentation()
           .AddPrometheusExporter()
           .AddOtlpExporter(options =>
           {
               string url = configuration["Otel:Endpoint"];
               options.Endpoint = new Uri(url);
           });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseAuthorization();
app.MapControllers();

app.Use(async (context, next) =>
{
    customCounter.Add(1);  // Increment custom counter for each request
    await next.Invoke();
});

app.Run();
