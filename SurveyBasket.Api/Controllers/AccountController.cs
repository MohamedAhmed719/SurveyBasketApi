using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Users;
using SurveyBasket.Api.Extensions;

namespace SurveyBasket.Api.Controllers
{
    [Route("me")]
    [ApiController]
    public class AccountController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> Info(CancellationToken cancellationToken)
        {
            var result = await _userService.GetProfileAsync(User.GetUserId()!, cancellationToken);

            return Ok(result.Value);
        }

        [HttpPut("info")]
        public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request)
        {
            var result = await _userService.UpdateProfileAsync(User.GetUserId()!,request);

            return NoContent();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
