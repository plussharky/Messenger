using System.ComponentModel.DataAnnotations;

namespace Messenger.Identity.Api.Dtos;

public sealed record LoginRequestDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
