namespace Messenger.Message.Application.Requests;

public sealed class CreateMessageRequest
{
    public string Text { get; init; } = string.Empty;
}
