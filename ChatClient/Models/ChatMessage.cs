namespace ChatClient.Models
{
    public class ChatMessage
    {
        public int Id { get; init; }
        public required string Text { get; init; }
        public DateTime SentAt { get; init; }

        public ChatMessage(string text)
        {
            Text = text;
            SentAt = DateTime.UtcNow;
        }

        private ChatMessage() { }
    }
}
