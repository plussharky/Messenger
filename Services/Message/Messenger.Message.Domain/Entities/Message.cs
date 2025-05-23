namespace Messenger.Message.Domain.Entities;

public sealed record Message
{
    private Message() // For EF Core
    {
        Text = string.Empty;
    }

    public Message(string text)
    {
        Text = text;
        SentAt = DateTime.UtcNow;
    }

    public int Id { get; init; }

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset SentAt { get; set; }
}
