using SurveyBasket.Api.Contracts.Answers;
using SurveyBasket.Api.Contracts.Common;
using SurveyBasket.Api.Contracts.Questions;
using System.Linq.Dynamic.Core;
namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context,ICacheService cacheService) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;

    private readonly ICacheService _cacheService = cacheService;

    private const string _cachePerfix = "avilableQuestions";
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var isPollExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!isPollExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var isQuestionExists = await _context.Questions.AnyAsync(x => x.PollId == pollId && x.Content == request.Content, cancellationToken);

        if (isQuestionExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

        var question = request.Adapt<Question>();

        question.PollId = pollId;

        await _context.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());

    }

    public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId,RequestFilters filter, CancellationToken cancellationToken = default)
    {
        var isPollExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!isPollExists)
            return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);

        ////var questions = await _context.Questions.Where(x => x.PollId == pollId).Include(x => x.Answers)
        ////    .Select(x => new QuestionResponse(x.Id, x.Content, x.Answers.Select(a => new AnswerResponse(a.Id, a.Content))).ToListAsync(cancellationToken);

        var query = _context.Questions.Where(x => x.PollId == pollId);

        if (!string.IsNullOrEmpty(filter.SearchValue))
            query = query.Where(x => x.Content.Contains(filter.SearchValue));

        if (!string.IsNullOrEmpty(filter.SortColumn))
            query = query.OrderBy($"{filter.SortColumn} {filter.SortDirection}");

         var soruce = query.Include(a => a.Answers)
             .AsNoTracking()
             .ProjectToType<QuestionResponse>();

        var questions = await PaginatedList<QuestionResponse>.CreateAsync(soruce, filter.PageNumber, filter.PageSize);

        return Result.Success(questions);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAvilableAsync(string userId,int pollId,CancellationToken cancellationToken)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

        var isPollExists = await _context.Polls.
            AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow),
            cancellationToken);

        if (!isPollExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await context.Questions.Where(x => x.IsActive && x.PollId == pollId)
            .Include(x => x.Answers)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
                )
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken); 

        return Result.Success(questions.Adapt<IEnumerable<QuestionResponse>>());
    }
    public async Task<Result<QuestionResponse>> GetAsync(int pollId,int questionId,CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.Where(x => x.PollId == pollId && x.Id == questionId)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.FirstOrDefaultAsync(x => x.PollId == pollId && x.Id == questionId, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);


        return Result.Success();
    }

    public async Task<Result> UpdateAsync(int pollId,int questionId,QuestionRequest request,CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.Where(x => x.PollId == pollId && x.Id == questionId)
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        var currentAnswers = question.Answers.Select(a => a.Content).ToList();

        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(a => question.Answers.Add(new Answer { Content = a }));

        question.Answers.ToList().ForEach(a =>
        {
            a.IsActive = request.Answers.Contains(a.Content);
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
