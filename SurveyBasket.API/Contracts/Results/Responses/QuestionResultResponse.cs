namespace SurveyBasket.API.Contracts.Results.Responses;

public record QuestionResultResponse(
    int Id,
    string Content,
    IEnumerable<AnswerResultResponse> Answers
);
