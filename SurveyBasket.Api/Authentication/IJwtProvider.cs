namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    (string Token, int ExpiresIn) GenerateJwtToken(ApplicationUser user,IEnumerable<string> roles,IEnumerable<string> permissions);
    string? ValidateToken(string Token);
}
