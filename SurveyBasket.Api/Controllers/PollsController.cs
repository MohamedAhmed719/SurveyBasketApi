using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Polls;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion(1)]
    [ApiVersion(2)]
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("")]
        [HasPermission(DefaultPermissions.GetPolls)]
        public async Task<IActionResult>GetAll(CancellationToken cancellationToken)
        {
            var polls = await _pollService.GetAllAsync(cancellationToken);
            
            return Ok(polls);
        }

        [HttpGet("current")]
        [EnableRateLimiting("userLimit")]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetCurrentV1(CancellationToken cancellationToken)
        {
            var polls = await _pollService.GetCurrentAsyncV1(cancellationToken);
            return Ok(polls);
        }

        [HttpGet("current")]
        [EnableRateLimiting("userLimit")]
        [MapToApiVersion(2)]
        public async Task<IActionResult> GetCurrentV2(CancellationToken cancellationToken)
        {
            var polls = await _pollService.GetCurrentAsyncV2(cancellationToken);
            return Ok(polls);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var pollResponse = await _pollService.GetAsync(id);
            
            return pollResponse.IsSuccess ? Ok(pollResponse.Value) : pollResponse.ToProblem();
        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] PollRequest request,CancellationToken cancellationToken)
        {
            var pollResult = await _pollService.AddAsync(request,cancellationToken);

            return pollResult.IsSuccess ? CreatedAtAction(nameof(Get), new { pollResult.Value!.Id }, pollResult.Value) : pollResult.ToProblem();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request,CancellationToken cancellationToken)
        {
            var isUpdated = await _pollService.UpdateAsync(id, request,cancellationToken);

            return isUpdated.IsSuccess ? NotFound() : isUpdated.ToProblem();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id,CancellationToken cancellationToken)
        {
            var isDeleted = await _pollService.DeleteAsync(id,cancellationToken);

            return isDeleted.IsSuccess ? Ok() : isDeleted.ToProblem();

        }

      

        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int id, CancellationToken cancellationToken)
        {
            var isDeleted = await _pollService.ToggleStatusAsync(id, cancellationToken);

            return isDeleted.IsSuccess ? NoContent() : isDeleted.ToProblem();

        }

    }
}
