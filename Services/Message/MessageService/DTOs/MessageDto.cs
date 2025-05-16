using MessageService.Data.Models;

namespace MessageService.DTOs;

public sealed record MessageDto(int Id, string Text, DateTime SentAt)
{
    internal static MessageDto FromMessage(Message message) =>
        new(message.Id, message.Text, message.SentAt);
} 