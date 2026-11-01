

namespace SurveyBasket.API.Contracts.Question.Responses;

public record QuestionResponse(
    int Id,
    string Content,
    bool IsActive,
    List<AnswerResponse> Answers
);
