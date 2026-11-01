namespace SurveyBasket.API.Contracts.Question.Requests;

public record QuestionRequest(string Content, List<string> Answers);

