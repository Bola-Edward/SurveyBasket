namespace SurveyBasket.API.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new Error("User.InvalidCredentials", "Invalid Email/Password", StatusCodes.Status401Unauthorized);
}
