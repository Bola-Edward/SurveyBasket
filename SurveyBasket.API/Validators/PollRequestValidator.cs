namespace SurveyBasket.API.Validators;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(x => x.StartsAt)
            .NotEmpty();

        RuleFor(x => x.EndsAt)
            .NotEmpty();

        RuleFor(x => x.EndsAt)
            .GreaterThanOrEqualTo(x => x.StartsAt);

        RuleFor(x => x.StartsAt)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
