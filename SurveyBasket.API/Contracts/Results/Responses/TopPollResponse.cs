namespace SurveyBasket.API.Contracts.Results.Responses;

public record TopPollResponse(
    int Id,
    string Title,
    int TotalVotes
);
