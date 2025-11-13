using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;

        [HttpGet("")]
        [HasPermission(DefaultPermissions.GetRoles)]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled,CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetAllAsync(cancellationToken, includeDisabled);

            return Ok(roles);
        }

        [HttpGet("{id}")]
        [HasPermission(DefaultPermissions.GetRoles)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var roleResult = await _roleService.GetAsync(id);

            return roleResult.IsSuccess ? Ok(roleResult.Value) : roleResult.ToProblem();
        }

        [HttpPost("")]
        [HasPermission(DefaultPermissions.AddRoles)]
        public async Task<IActionResult> Add([FromBody] CreateRoleRequest request)
        {
            var roleResult = await _roleService.AddAsync(request);

            return roleResult.IsSuccess ? CreatedAtAction(nameof(Get),new {roleResult.Value.Id},roleResult.Value) : roleResult.ToProblem();
        }

        [HttpPut("{id}")]
        [HasPermission(DefaultPermissions.UpdateRoles)]
        public async Task<IActionResult> Update([FromRoute] string id,[FromBody] CreateRoleRequest request)
        {
            var roleResult = await _roleService.UpdateAsync(id,request);

            return roleResult.IsSuccess ? NoContent() : roleResult.ToProblem();
        }

        [HttpPut("{id}/toggle-status")]
        [HasPermission(DefaultPermissions.UpdateRoles)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string id)
        {
            var roleResult = await _roleService.ToggleStatusAsync(id);

            return roleResult.IsSuccess ? NoContent() : roleResult.ToProblem();
        }
    }
}
