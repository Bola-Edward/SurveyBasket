namespace SurveyBasket.API.Contracts.Authentication.Requests;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);
