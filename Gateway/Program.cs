using Core.Options;
using Core.ServiceCollectionExtensions;
using WindowsApi.ServiceCollectionExtensions;
using Gateway.ServiceCollectionExtensions;
using Gateway;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCore();
builder.Services.AddWindowsApi();
builder.Services.AddGateway();

builder.AddProcessNameConfigurationSource();

builder.Services.Configure<StepsOptions>(builder.Configuration.GetSection(StepsOptions.SectionName));

builder.Services.AddHostedService<EntrypointHostedService>();
builder.Services.AddHostedService<UiHostedService>();

var host = builder.Build();
await host.RunAsync();