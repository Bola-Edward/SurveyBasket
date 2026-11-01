
namespace SurveyBasket.API.Contracts.Question.Validators;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Answers)
            .NotEmpty()
            .Must(answers => answers.Count >= 2)
            .WithMessage("A question must have at least two answers.");

        RuleForEach(x => x.Answers)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Answers)
            .Must(answers => answers.Distinct(StringComparer.OrdinalIgnoreCase).Count() == answers.Count)
            .WithMessage("Duplicate answers are not allowed.");
    }
}
