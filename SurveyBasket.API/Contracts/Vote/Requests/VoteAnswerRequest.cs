namespace SurveyBasket.API.Contracts.Vote.Requests;

public record VoteAnswerRequest(
    int QuestionId,
    int AnswerId
);

