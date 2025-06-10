using System.ComponentModel.DataAnnotations;

namespace Messenger.Identity.Api.Dtos;

public sealed class RegisterRequestDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
