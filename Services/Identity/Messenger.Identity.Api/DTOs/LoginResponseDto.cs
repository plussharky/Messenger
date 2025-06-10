namespace Messenger.Identity.Api.Dtos;

public sealed record LoginResponseDto
{
    public required string AccessToken { get; set; }

    public required string RefreshToken { get; set; }
}
