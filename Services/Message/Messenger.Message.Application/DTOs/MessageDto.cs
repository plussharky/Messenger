using System;

namespace Messenger.Message.Application.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
} 