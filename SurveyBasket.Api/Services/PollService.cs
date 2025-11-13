using Hangfire;
using SurveyBasket.Api.Contracts.Polls;
using System.Collections.Immutable;
namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context,INotificationService notificationService) : IPollService
{
    private readonly ApplicationDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken=default) => await _context.Polls.
        AsNoTracking().ProjectToType<PollResponse>().ToListAsync(cancellationToken);

    //public Poll? Get(int id) => _polls.SingleOrDefault(x => x.Id == id);
    public async Task<Result<PollResponse>> AddAsync(PollRequest request,CancellationToken cancellationToken=default)
    {

        var isTitleExists = await _context.Polls.AnyAsync(x => x.Title == request.Title, cancellationToken);

        if (isTitleExists)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);

        var poll = request.Adapt<Poll>();

        await _context.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result<PollResponse>> GetAsync(int id)
    {
        var poll = await _context.Polls.FindAsync(id);

        if (poll is null)
            return Result.Failure<PollResponse>(PollErrors.PollNotFound);

        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, PollRequest request,CancellationToken cancellationToken=default)
    {
        var currentPoll = await _context.Polls.FindAsync(id);
        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        currentPoll.Title = request.Title;
        currentPoll.Summary = request.Summary;
        currentPoll.StartsAt = request.StartsAt;
        currentPoll.EndsAt = request.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id,CancellationToken cancellationToken=default)
    {
        var poll = await _context.Polls.FindAsync(id);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        _context.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);
        poll.IsPublished = !poll.IsPublished;

        if (poll.IsPublished && poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
            BackgroundJob.Enqueue(() => _notificationService.SendNewPollNotification(id));

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<IEnumerable<PollResponse>> GetCurrentAsyncV1(CancellationToken cancellationToken = default)
    {
        var polls = await _context.Polls
            .Where(x => x.IsPublished &&  x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);

        return polls;
    }
    public async Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default)
    {
        var polls = await _context.Polls
            .Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .AsNoTracking()
            .ProjectToType<PollResponseV2>()
            .ToListAsync(cancellationToken);

        return polls;
    }
}
