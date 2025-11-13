namespace SurveyBasket.Api.Contracts.Authentication;

public class ForgretPasswordValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgretPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
