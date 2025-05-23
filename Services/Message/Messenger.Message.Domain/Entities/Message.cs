namespace Messenger.Messages.Domain.Entities;

public sealed record Message
{
    private Message() // For EF Core
    {
        Text = string.Empty;
    }

    public Message(string text, DateTimeOffset sentAt)
    {
        Text = text;
        SentAt = sentAt;
    }

    public int Id { get; init; }

    public string Text { get; set; } = string.Empty;

    required public DateTimeOffset SentAt { get; set; }
}
