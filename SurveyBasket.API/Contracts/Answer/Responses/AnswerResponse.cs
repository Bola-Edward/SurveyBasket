namespace SurveyBasket.API.Contracts.Answer.Responses;

public record AnswerResponse(
    int Id,
    string Content,
    bool IsActive
);