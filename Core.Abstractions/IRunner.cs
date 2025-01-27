namespace Core.Abstractions;

public interface IRunner
{
    Task<Models.RunnerResult> RunToCompletion(CancellationToken token);
}