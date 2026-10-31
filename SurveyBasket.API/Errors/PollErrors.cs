namespace SurveyBasket.API.Errors;

public static class PollErrors
{
    public static readonly Error NotFound =
        new("Poll.NotFound", "Poll was not found", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateTitle =
        new("Poll.DuplicateTitle", "A poll with the same title already exists", StatusCodes.Status409Conflict);
}