namespace SurveyBasket.API.Contracts.Results.Responses;

public record VotePerDayResponse(
    DateOnly Date,
    int TotalVotes
);
