using Core.Abstractions.Events.Stages;
using OneOf;

namespace Core.Abstractions.Events;

[GenerateOneOf]
public partial class RunEvent : OneOfBase<
    SetupEvent.Started,
    StartAppEvent.Started,
    StartAppEvent.ClickMade,
    RunTimerEvent.Started,
    RunTimerEvent.TimerUpdated,
    RunShutdownTimerEvent.Started,
    RunShutdownTimerEvent.TimerUpdated,
    StopAppEvent.Started,
    StopAppEvent.ClickMade,
    FinishedEvent.Started
>;