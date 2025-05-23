namespace Messenger.Message.Application.Services;

internal sealed class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset GetCurrentTime()
    {
        return DateTimeOffset.UtcNow;
    }
}
