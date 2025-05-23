namespace Messenger.Messages.Api.Requests;

public sealed class CreateMessageRequestDto
{
    public string Text { get; init; } = string.Empty;
}
