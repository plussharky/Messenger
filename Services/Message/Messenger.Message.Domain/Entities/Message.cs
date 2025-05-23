namespace Messenger.Messages.Domain.Entities;

public sealed record Message
{
    public Message(string text, DateTimeOffset sentAt)
    {
        Text = text;
        SentAt = sentAt;
    }

    public int Id { get; init; }

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset SentAt { get; set; }
}
