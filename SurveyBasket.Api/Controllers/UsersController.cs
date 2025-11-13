using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("")]
        [HasPermission(DefaultPermissions.GetUsers)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [HasPermission(DefaultPermissions.GetUsers)]
        public async Task<IActionResult> Get([FromRoute] string userId)
        {
            var userResult = await _userService.GetAsync(userId);

            return userResult.IsSuccess ? Ok(userResult.Value) : userResult.ToProblem();
        }

        [HttpPost("")]
        [HasPermission(DefaultPermissions.AddUsers)]
        public async Task<IActionResult> Add([FromBody] CreateUserRequest request)
        {
            var result = await _userService.AddAsync(request);

            return result.IsSuccess ? CreatedAtAction(nameof(Get),new { result.Value!.Id}, result.Value) : result.ToProblem();
        }


        [HttpPut("{id}")]
        [HasPermission(DefaultPermissions.UpdateUsers)]
        public async Task<IActionResult> Update([FromRoute] string id,[FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateAsync(id, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{id}/toggle-status")]
        [HasPermission(DefaultPermissions.UpdateUsers)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string id)
        {
            var result = await _userService.ToggleStatusAsync(id);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{id}/unlock")]
        [HasPermission(DefaultPermissions.UpdateUsers)]
        public async Task<IActionResult> Unlock([FromRoute] string id)
        {
            var result = await _userService.UnlockAsync(id);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
