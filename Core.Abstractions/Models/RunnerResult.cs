using OneOf;

namespace Core.Abstractions.Models;

[GenerateOneOf]
public partial class RunnerResult : OneOfBase<
    RunnerResult.Success, 
    RunnerResult.Shutdown, 
    RunnerResult.Error,
    Exception
>
{
    public record Success;
    
    public record Shutdown;

    public enum Error
    {
        AdminRightsRequired,
        CantFindApplication
    }
}





