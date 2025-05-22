namespace ChatClient.Models;

internal sealed class ChatMessage
{
    public ChatMessage(string text)
    {
        Text = text;
        SentAt = DateTimeOffset.UtcNow;
    }

    private ChatMessage()
    {
    }

    public int Id { get; init; }

    required public string Text { get; init; }

    public DateTimeOffset SentAt { get; init; }
}
