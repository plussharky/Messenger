namespace Messenger.Common.Services;

public sealed class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset GetCurrentTime() => DateTimeOffset.UtcNow;
}
