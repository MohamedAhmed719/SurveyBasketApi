namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetJwtAsync(string email, string password);
    Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken);
    Task<Result> RegisterAsync(RegisterRequest request);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmEmailCodeAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> SendResetPasswordCodeAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
}
