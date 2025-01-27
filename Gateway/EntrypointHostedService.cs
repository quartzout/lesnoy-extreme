using Core;
using Microsoft.Extensions.Hosting;

namespace Gateway;

public class EntrypointHostedService(Runner macroStep) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await macroStep.RunToCompletion(stoppingToken);
    }
}