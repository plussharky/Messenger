namespace Messenger.Message.Application.Services;

internal interface ITimeProvider
{
    public DateTimeOffset GetCurrentTime();
}
