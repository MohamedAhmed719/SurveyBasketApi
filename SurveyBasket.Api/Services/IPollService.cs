using SurveyBasket.Api.Contracts.Polls;

namespace SurveyBasket.Api.Services;

public interface IPollService
{
    Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<PollResponse>> GetCurrentAsyncV1(CancellationToken cancellationToken = default);
    Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> GetAsync(int id);
    Task<Result> UpdateAsync(int id, PollRequest request,CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id,CancellationToken cancellationTokenn=default);
    Task<Result> ToggleStatusAsync(int id,CancellationToken cancellationTokenn=default);
}
