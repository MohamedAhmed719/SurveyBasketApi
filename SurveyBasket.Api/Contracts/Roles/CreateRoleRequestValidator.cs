namespace SurveyBasket.Api.Contracts.Roles;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Permissions)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Permissions)
            .Must(x => x.Distinct().Count() == x.Count())
            .WithMessage("you can't add duplicated permissions for the same role")
            .When(x => x.Permissions is not null);
    }
}
