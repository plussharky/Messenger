namespace Messenger.Message.Domain.Entities;

public sealed record MessageEntity
{
    private MessageEntity() // For EF Core
    {
        Text = string.Empty;
    }

    public MessageEntity(string text)
    {
        Text = text;
        SentAt = DateTime.UtcNow;
    }

    public int Id { get; init; }

    public string Text { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }
}
