namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static Error InvalidCredentials = new("User.InvalidCredentials", "Invalid email/password",StatusCodes.Status400BadRequest);
    public static Error DuplicatedEmail = new("User.DuplicatedEmail", "Another user with the same email is already exists",StatusCodes.Status409Conflict);
    public static Error EmailNotConfirmed = new("User.EmailNotConfirmed", "Email not Confirmed",StatusCodes.Status400BadRequest);
    public static Error InvalidCode = new("User.InvalidCode", "Invalid Code",StatusCodes.Status400BadRequest);
    public static Error DuplicatedEmailConfirmed = new("User.DuplicatedEmailConfirmed", "Email is already confirmed",StatusCodes.Status409Conflict);
    public static Error UserLockedOut = new("User.UserLockedOut", "User is locked out please contact your adminstrator",StatusCodes.Status400BadRequest);
    public static Error DisabledUser = new("User.DisabledUser", "Disabled User please contact your adminstrator", StatusCodes.Status400BadRequest);
    public static Error UserNotFound = new("User.UserNotFound", "User not found", StatusCodes.Status404NotFound);
    public static Error InvalidRoles = new("User.InvalidRoles", "User invalid roles", StatusCodes.Status400BadRequest);
}
