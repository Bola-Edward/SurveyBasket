namespace SurveyBasket.API.Errors;

public static class PollErrors
{
    public static readonly Error NotFound =
        new("Poll.NotFound", "Poll was not found", StatusCodes.Status404NotFound);
}