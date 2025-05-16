namespace MessageService.Data.Models;

public sealed class Message
{
    public int Id { get; init; }
    public string Text { get; init; }
    public DateTime SentAt { get; init; }

    private Message() // For EF Core
    { 
        Text = string.Empty;
    } 

    public Message(string text)
    {
        Text = text;
        SentAt = DateTime.UtcNow;
    }
}
