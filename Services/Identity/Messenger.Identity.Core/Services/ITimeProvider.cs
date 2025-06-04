namespace Messenger.Identity.Core.Services;

public interface ITimeProvider
{
    DateTimeOffset GetCurrentTime();
}
