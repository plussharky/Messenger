using static System.Net.Mime.MediaTypeNames;

namespace Messenger.Message.Domain.Entities;

public class MessageEntity
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }

    private MessageEntity() // For EF Core
    {
        Text = string.Empty;
    }

    public MessageEntity(string text)
    {
        Text = text;
        SentAt = DateTime.UtcNow;
    }
}