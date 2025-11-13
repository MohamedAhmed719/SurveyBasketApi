namespace SurveyBasket.Api.Contracts.Roles;

public record CreateRoleRequest(string Name,IList<string> Permissions);