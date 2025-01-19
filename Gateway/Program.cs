using Gateway;
using Gateway.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steps;
using Steps.Options;
using WindowManager;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSteps();
builder.Services.AddWindowManager();
builder.Services.AddGateway();

builder.AddProcessNameConfigurationSource();

builder.Services.Configure<StepsOptions>(builder.Configuration.GetSection(StepsOptions.SectionName));

builder.Services.AddHostedService<EntrypointHostedService>();

var host = builder.Build();
await host.RunAsync();