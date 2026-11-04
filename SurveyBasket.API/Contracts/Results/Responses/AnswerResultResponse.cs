namespace SurveyBasket.API.Contracts.Results.Responses;

public record AnswerResultResponse(
    int Id,
    string Content,
    int Votes,
    double Percentage
);
