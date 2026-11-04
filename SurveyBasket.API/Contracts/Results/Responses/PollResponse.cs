namespace SurveyBasket.API.Contracts.Results.Responses;

public record PollResponse(
    int Id,
    string Title,
    int TotalVotes
);
