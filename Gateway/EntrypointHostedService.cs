using Microsoft.Extensions.Hosting;
using Steps;

namespace Gateway;

public class EntrypointHostedService(MarcoStep macroStep) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await macroStep.Run(stoppingToken);
    }
}