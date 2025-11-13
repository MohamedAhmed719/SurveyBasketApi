namespace SurveyBasket.Api.Contracts.Votes;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {

        RuleFor(x => x.Answers)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Distinct().Count() == x.Count())
            .WithMessage("you can't add duplicated answer for the same question");

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(v=>v.Add(new VoteAnswerRequestValidator()));
    }
}
