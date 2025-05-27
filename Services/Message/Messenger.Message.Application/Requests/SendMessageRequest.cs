namespace Messenger.Messages.Application.Requests;

public sealed class SendMessageRequest
{
    required public Guid Id { get; set; }

    public string Text { get; init; } = string.Empty;
}
