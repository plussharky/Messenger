namespace ChatClient.Models;

internal sealed class ChatMessage
{
    public ChatMessage(string text)
    {
        Text = text;
        SentAt = DateTime.UtcNow;
    }

    private ChatMessage()
    {
    }

    public int Id { get; init; }

    required public string Text { get; init; }

    public DateTime SentAt { get; init; }
}
