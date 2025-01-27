using Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Gateway;

public class UiHostedService(
    IRunEventConsumer eventConsumer,
    IRunEventUiHandler runEventHandler,
    ILogger<UiHostedService> logger) 
    
    : ParallelBackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var runEvent in eventConsumer.ConsumeAsync(stoppingToken))
        {
            try
            {
                await runEventHandler.Handle(runEvent);
            }
            catch (Exception exp)
            {
                logger.LogError("Error handling RunEvent: {message}. InnerException: {innerMessage}",
                    exp.Message, exp.InnerException?.Message);
            }
            
        }
    }
}