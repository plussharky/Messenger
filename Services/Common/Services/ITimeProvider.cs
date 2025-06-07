namespace Messenger.Common.Services;

public interface ITimeProvider
{
    DateTimeOffset GetCurrentTime();
}
