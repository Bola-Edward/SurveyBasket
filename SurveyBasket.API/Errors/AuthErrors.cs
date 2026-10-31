namespace SurveyBasket.API.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Invalid email or password", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidToken =
        new("Auth.InvalidToken", "The access token is invalid", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("Auth.InvalidRefreshToken", "The refresh token is invalid or expired", StatusCodes.Status401Unauthorized);

    public static readonly Error RefreshTokenUpdateFailed =
        new("Auth.RefreshTokenUpdateFailed", "Failed to update refresh token", StatusCodes.Status500InternalServerError);
}
