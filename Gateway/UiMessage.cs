using MediatR;

namespace Gateway;

// Создается рефлексией, необходим конструктор
public record UiMessage<TPayload>(TPayload Payload) : IRequest;