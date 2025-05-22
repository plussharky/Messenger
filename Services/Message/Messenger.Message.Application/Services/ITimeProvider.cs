namespace Messenger.Message.Application.Services;

public interface ITimeProvider
{
    public DateTimeOffset GetCurrentTime();
}
