namespace Messenger.Message.API.Requests;

public sealed class CreateMessageRequest
{
    public string Text { get; init; } = string.Empty;
}
