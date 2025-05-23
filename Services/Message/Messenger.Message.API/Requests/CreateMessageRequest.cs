namespace Messenger.Message.Api.Requests;

public sealed class CreateMessageRequest
{
    public string Text { get; init; } = string.Empty;
}
