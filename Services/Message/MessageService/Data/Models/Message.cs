namespace MessageService.Data.Models;

public class Message
{
    public int Id { get; init; }
    public required string Text { get; init; }
    public DateTime SentAt { get; init; }

    private Message() { } // For EF Core

    public Message(string text)
    {
        Text = text;
        SentAt = DateTime.UtcNow;
    }
}
