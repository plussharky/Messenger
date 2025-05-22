namespace Messenger.Message.Application.Services;

public sealed class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset GetCurrentTime()
    {
        return DateTimeOffset.UtcNow;
    }
}
