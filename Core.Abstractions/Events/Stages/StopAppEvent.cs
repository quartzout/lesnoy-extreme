using OneOf;

namespace Core.Abstractions.Events.Stages;

public static class StopAppEvent
{
    public record Started;

    public record ClickMade;

    public record Finished;
}