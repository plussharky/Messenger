using System.Globalization;
using Messenger.Messages.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Messenger.Messages.ComponentTests;

public sealed class TestTimeProvider(IHttpContextAccessor httpContextAccessor)
    : ITimeProvider
{
    private const string CurrentTimeHeader = "X-Current-Time";

    private static readonly IFormatProvider FormatProvider = CultureInfo.InvariantCulture;

    public DateTimeOffset GetCurrentTime()
    {
        var context = httpContextAccessor.HttpContext;
        if (context?.Request.Headers.TryGetValue(CurrentTimeHeader, out var timeHeader) != true)
        {
            return DateTimeOffset.UtcNow;
        }

        return DateTimeOffset.TryParse(timeHeader, FormatProvider, DateTimeStyles.None, out var time)
            ? time
            : DateTimeOffset.UtcNow;
    }
}
