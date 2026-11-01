namespace SurveyBasket.API.Errors;

public static class QuestionErrors
{
    public static readonly Error NotFound =
        new("Questions.NotFound", "Question was not found.", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateContent =
        new("Questions.DuplicateContent", "A question with the same content already exists in this poll.", StatusCodes.Status409Conflict);

    public static readonly Error PollNotFound =
        new("Questions.PollNotFound", "Poll was not found.", StatusCodes.Status404NotFound);
}
