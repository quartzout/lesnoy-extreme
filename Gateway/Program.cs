using Core;
using Core.Options;
using Gateway;
using Gateway.Extensions;
using Infrastructure.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCore();
builder.Services.AddInfrastructureWindows();
builder.Services.AddGateway();

builder.AddProcessNameConfigurationSource();

builder.Services.Configure<StepsOptions>(builder.Configuration.GetSection(StepsOptions.SectionName));

builder.Services.AddHostedService<EntrypointHostedService>();

var host = builder.Build();
await host.RunAsync();