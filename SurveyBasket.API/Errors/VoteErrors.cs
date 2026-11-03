namespace SurveyBasket.API.Errors;

public static class VoteErrors
{
    public static readonly Error AlreadyVoted =
        new("Poll.AlreadyVoted", "You have already voted in this poll.", StatusCodes.Status409Conflict);
    public static readonly Error InvalidQuestions =
        new("Vote.InvalidQuestions", "The submitted questions do not match the available questions in this poll.", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidAnswers =
        new("Vote.InvalidAnswers", "One or more selected answers do not belong to their corresponding questions.", StatusCodes.Status400BadRequest);
}
