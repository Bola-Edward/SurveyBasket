namespace SurveyBasket.API.Contracts.Vote.Requests;

public record VoteRequest(
    IEnumerable<VoteAnswerRequest> VoteAnswers
);

