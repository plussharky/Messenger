namespace MessageService.Data.Models;

public class Message
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public DateTime SentAt { get; set; }
}
