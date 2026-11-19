namespace SurveyBasket.API.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new Error("User.InvalidCredentials", "Invalid Email/Password", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmail =
        new Error("User.DuplicatedEmail", "Another user with the same email already exists", StatusCodes.Status409Conflict);

    public static readonly Error NotFound =
        new("User.NotFound", "User not found", StatusCodes.Status404NotFound);

    public static readonly Error EmailAlreadyConfirmed =
        new("User.EmailAlreadyConfirmed", "Email is already confirmed", StatusCodes.Status409Conflict);

    public static readonly Error InvalidEmailConfirmationCode =
        new("User.InvalidEmailConfirmationCode", "Invalid or expired email confirmation code", StatusCodes.Status400BadRequest);
}
