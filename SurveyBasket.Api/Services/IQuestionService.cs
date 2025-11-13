using SurveyBasket.Api.Contracts.Common;
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public interface IQuestionService
{
    Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters filter, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAvilableAsync(string userId, int pollId, CancellationToken cancellationToken);
    Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default);
}
