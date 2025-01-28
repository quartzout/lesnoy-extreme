using Core.Abstractions.Events.Stages;
using OneOf;

namespace Core.Abstractions.Events;

[GenerateOneOf]
public partial class RunEvent : OneOfBase<
    SetupEvent.Started,
    SetupEvent.Finished,
    StartAppEvent.Started,
    StartAppEvent.ClickMade,
    StartAppEvent.Finished,
    RunTimerEvent.Started,
    RunTimerEvent.TimerUpdated,
    RunTimerEvent.Finished,
    RunShutdownTimerEvent.Started,
    RunShutdownTimerEvent.TimerUpdated,
    RunShutdownTimerEvent.Finished,
    StopAppEvent.Started,
    StopAppEvent.ClickMade,
    StopAppEvent.Finished,
    FinishedEvent.Started
>;