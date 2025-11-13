namespace SurveyBasket.Api.Contracts.Authentication;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.userId).NotEmpty();
    }
}
