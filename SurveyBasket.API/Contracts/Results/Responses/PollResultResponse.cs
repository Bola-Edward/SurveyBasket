namespace SurveyBasket.API.Contracts.Results.Responses;

public record PollResultResponse(
    int Id,
    string Title,
    string Summary,
    int TotalVotes,
    IEnumerable<QuestionResultResponse> Questions
);
