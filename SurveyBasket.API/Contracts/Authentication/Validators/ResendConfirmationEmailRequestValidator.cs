using ResendConfirmationEmailRequest = SurveyBasket.API.Contracts.Authentication.Requests.ResendConfirmationEmailRequest;

namespace SurveyBasket.API.Contracts.Authentication.Validators;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
