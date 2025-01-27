using Core;
using Core.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Gateway;

public class EntrypointHostedService(IRunner runner) : ParallelBackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await runner.RunToCompletion(stoppingToken);
    }
}