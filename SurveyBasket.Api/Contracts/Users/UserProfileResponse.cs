namespace SurveyBasket.Api.Contracts.Users;

public record UserProfileResponse
    (
    string Id,
    string UserName,
    string FirstName,
    string LastName,
    string Email);
