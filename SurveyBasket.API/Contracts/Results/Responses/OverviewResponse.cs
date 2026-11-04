namespace SurveyBasket.API.Contracts.Results.Responses;

public record OverviewResponse(
    int TotalPolls,
    int TotalVotes,
    int TotalQuestions,
    IEnumerable<PollResponse> Polls
);
