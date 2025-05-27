namespace Messenger.Messages.Domain.Entities;

public sealed record Message
{
    public Message(Guid id, string text, DateTimeOffset sentAt)
    {
        Id = id;
        Text = text;
        SentAt = sentAt;
    }

    public Guid Id { get; init; }

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset SentAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; } = null;
}
