using Microsoft.AspNetCore.RateLimiting;


namespace SurveyBasket.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableRateLimiting("ipLimit")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var authResult = await _authService.GetJwtAsync(request.Email, request.Password);

            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _authService.GetRefreshTokenAsync(request.Token,request.RefreshToken);

            return authResponse is null ? BadRequest("Invalid user/email") : Ok(authResponse);
        }

        [HttpPut("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
        {
            var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken);

            return isRevoked ? BadRequest("Invalid user/email") : Ok();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        [HttpPost("Confirm-Email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(request);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        [HttpPost("resend-Confirm-Email")]
        public async Task<IActionResult> ResendEmailConfirmationCode([FromBody] ResendEmailConfirmationCodeRequest request)
        {
            var result = await _authService.ResendConfirmEmailCodeAsync(request.Email);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await _authService.SendResetPasswordCodeAsync(request.Email);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }
    }
}
