namespace SurveyBasket.API.Contracts.Poll.Requests;

public record PollRequest(
    string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
);
