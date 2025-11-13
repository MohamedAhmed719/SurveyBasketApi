using System.Runtime.CompilerServices;

namespace SurveyBasket.Api.Contracts.Polls;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .Length(3, 1500);

        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.EndsAt)
           .NotEmpty();

        RuleFor(x => x)
            .Must(HasValidDate);
    }


    private bool HasValidDate(PollRequest request)
    {
        return request.EndsAt >= request.StartsAt;
    }
}
