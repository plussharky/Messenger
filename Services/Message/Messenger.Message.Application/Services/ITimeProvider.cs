namespace Messenger.Messages.Application.Services;

internal interface ITimeProvider
{
    DateTimeOffset GetCurrentTime();
}
