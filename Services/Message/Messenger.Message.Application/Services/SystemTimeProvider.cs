namespace Messenger.Messages.Application.Services;

internal sealed class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset GetCurrentTime()
    {
        return DateTimeOffset.UtcNow;
    }
}
