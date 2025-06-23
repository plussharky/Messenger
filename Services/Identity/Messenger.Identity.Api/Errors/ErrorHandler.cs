using Messenger.Identity.Core.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Identity.Api.Errors;

public sealed class ErrorHandler(IHttpContextAccessor httpContextAccessor)
    : IErrorHandler
{
    private const string RegistrationErrorTitle = "Ошибка регистрации";
    private const string LoginErrorTitle = "Ошибка входа";
    private const string RefreshTokenErrorTitle = "Ошибка обновления токена";
    private const string UnknownErrorTitle = "Неизвестная ошибка";

    public IActionResult Handle(RegisterError error)
    {
        var (statusCode, title, detail) = error switch
        {
            RegisterError.EmailAlreadyExists => (
                StatusCodes.Status409Conflict,
                RegistrationErrorTitle,
                "Пользователь с таким email уже существует."),
            _ => (
                StatusCodes.Status500InternalServerError,
                UnknownErrorTitle,
                "Произошла непредвиденная ошибка во время регистрации."),
        };

        return CreateProblemResult(statusCode, title, detail);
    }

    public IActionResult Handle(LoginError error)
    {
        var (statusCode, title, detail) = error switch
        {
            LoginError.EmailNotFound => (
                StatusCodes.Status400BadRequest,
                LoginErrorTitle,
                "Пользователь с указанным email не найден."),
            LoginError.InvalidPassword => (
                StatusCodes.Status400BadRequest,
                LoginErrorTitle,
                "Неверный пароль."),
            LoginError.TokenGenerationFailed => (
                StatusCodes.Status500InternalServerError,
                LoginErrorTitle,
                "Произошла ошибка при создании токена."),
            _ => (
                StatusCodes.Status500InternalServerError,
                UnknownErrorTitle,
                "Произошла непредвиденная ошибка во время входа."),
        };
        return CreateProblemResult(statusCode, title, detail);
    }

    public IActionResult Handle(RefreshTokenError error)
    {
        var (statusCode, title, detail) = error switch
        {
            RefreshTokenError.InvalidToken => (
                StatusCodes.Status400BadRequest,
                RefreshTokenErrorTitle,
                "Невалидный токен."),
            RefreshTokenError.TokenExpired => (
                StatusCodes.Status400BadRequest,
                RefreshTokenErrorTitle,
                "Срок действия токена истек."),
            RefreshTokenError.UserNotFound => (
                StatusCodes.Status400BadRequest,
                RefreshTokenErrorTitle,
                "Пользователь не найден."),
            RefreshTokenError.TokenNotFound => (
                StatusCodes.Status400BadRequest,
                RefreshTokenErrorTitle,
                "Токен не найден."),
            RefreshTokenError.TokenRevoked => (
                StatusCodes.Status400BadRequest,
                RefreshTokenErrorTitle,
                "Токен был отозван."),
            _ => (
                StatusCodes.Status500InternalServerError,
                UnknownErrorTitle,
                "Произошла непредвиденная ошибка во время обновления токена."),
        };
        return CreateProblemResult(statusCode, title, detail);
    }

    private IActionResult CreateProblemResult(
        int statusCode,
        string title,
        string detail)
    {
        var context = httpContextAccessor.HttpContext;
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context?.Request.Path,
        };
        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode,
        };
    }
}
