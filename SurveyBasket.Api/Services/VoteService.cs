using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddAsync(int pollId,string userId,VoteRequest voteRequest,CancellationToken cancellationToken = default)
    {

        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure(VoteErrors.DuplicatedVote);

        var isPollExists = await _context.Polls.
            AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!isPollExists)
            return Result.Failure(PollErrors.PollNotFound);

        await _context.AddAsync(voteRequest.Adapt<Vote>());

        var avilableQuestions = await _context.Questions.Where(x=>x.PollId == pollId && x.IsActive).Select(x => x.Id).ToListAsync(cancellationToken);

        if (voteRequest.Answers.Select(x => x.QuestionId).SequenceEqual(avilableQuestions))
            return Result.Failure(VoteErrors.InvalidQuestions);

        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = voteRequest.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.AddAsync(vote,cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
