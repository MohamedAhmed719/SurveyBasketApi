using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.FirstName).Length(3, 100);
        RuleFor(x => x.LastName).Length(3, 100);

        RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(RegexPattern.Password)
                .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");

    }
}
