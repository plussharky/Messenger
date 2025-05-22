namespace Messenger.Message.Application.DTOs;

public sealed class MessageDto
{
    public int Id { get; init; }

    public string Text { get; init; } = string.Empty;

    public DateTimeOffset SentAt { get; init; }
}
