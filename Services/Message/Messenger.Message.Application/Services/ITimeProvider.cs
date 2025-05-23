namespace Messenger.Message.Application.Services;

internal interface ITimeProvider
{
    DateTimeOffset GetCurrentTime();
}
