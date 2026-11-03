using SurveyBasket.API.Contracts.Vote.Requests;

namespace SurveyBasket.API.Contracts.Vote.Validators;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x => x.VoteAnswers)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.VoteAnswers)
            .SetValidator(new VoteAnswerRequestValidator());

        RuleFor(x => x.VoteAnswers)
            .Must(answers => answers
                .Select(x => x.QuestionId)
                .Distinct()
                .Count() == answers.Count())
            .When(x => x.VoteAnswers is not null)
            .WithMessage("Each question must have only one selected answer.");
    }
}
