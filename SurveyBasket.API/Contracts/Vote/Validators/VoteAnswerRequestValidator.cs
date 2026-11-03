using SurveyBasket.API.Contracts.Vote.Requests;

namespace SurveyBasket.API.Contracts.Vote.Validators;

public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
{
    public VoteAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0);

        RuleFor(x => x.AnswerId)
            .GreaterThan(0);
    }
}