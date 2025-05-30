namespace Messenger.Messages.Application.Requests;

public sealed class SendMessageRequest
{
    public required Guid Id { get; set; }

    public string Text { get; init; } = string.Empty;
}
