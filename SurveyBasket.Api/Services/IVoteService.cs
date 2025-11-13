using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services;

public interface IVoteService
{
    Task<Result> AddAsync(int pollId, string userId, VoteRequest voteRequest, CancellationToken cancellationToken = default);
}
