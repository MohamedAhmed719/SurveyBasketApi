namespace SurveyBasket.Api.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound = new Error("Role.NotFound", "There was no role with the given id", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedRoleName = new Error("Role.DuplicatedRoleName", "another role with the same name is already exists", StatusCodes.Status409Conflict);
    public static readonly Error InvalidPermission = new Error("Role.InvalidPermission", "Invalid permissions", StatusCodes.Status400BadRequest);
}
