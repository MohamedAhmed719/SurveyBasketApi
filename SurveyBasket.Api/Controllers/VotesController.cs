using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Contracts.Votes;
using SurveyBasket.Api.Extensions;
using System.Security.Claims;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{pollId}/vote")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("concurrency")]
    public class VotesController(IQuestionService questionService,IVoteService voteService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;
        private readonly IVoteService _voteService = voteService;

        [HttpGet("")]
        public async Task<IActionResult> Start([FromRoute] int pollId,CancellationToken cancellationToken)
        {
            
            var questionsResult = await _questionService.GetAllAvilableAsync(User.GetUserId()!, pollId, cancellationToken);

            return questionsResult.IsSuccess ? Ok(questionsResult.Value) : questionsResult.ToProblem();
        }

        [HttpPost("")]
        public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request,CancellationToken cancellationToken)
        {
            var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);

            return result.IsSuccess ? Created() : result.ToProblem();
        } 
    }
}
