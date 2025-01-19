using Gateway.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Steps;
using WindowManager;

var services = new ServiceCollection();

services.AddSteps();
services.AddWindowManager();
services.AddGateway();

services.AddSolutionOptions();

var serviceProvider = services.BuildServiceProvider();

var scope = serviceProvider.CreateScope();
var entryPoint = scope.ServiceProvider.GetRequiredService<MarcoStep>();
await entryPoint.Run();