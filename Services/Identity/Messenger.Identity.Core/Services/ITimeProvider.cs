namespace Messenger.Identity.Core.Services;

internal interface ITimeProvider
{
    DateTimeOffset GetCurrentTime();
}
