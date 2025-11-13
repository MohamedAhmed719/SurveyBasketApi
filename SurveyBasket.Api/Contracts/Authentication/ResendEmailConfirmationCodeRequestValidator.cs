namespace SurveyBasket.Api.Contracts.Authentication;

public class ResendEmailConfirmationCodeRequestValidator : AbstractValidator<ResendEmailConfirmationCodeRequest>
{
    public ResendEmailConfirmationCodeRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
