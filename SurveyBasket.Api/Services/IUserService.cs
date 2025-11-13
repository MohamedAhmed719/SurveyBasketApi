using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<Result<UserResponse>> GetAsync(string userId);
    Task<Result<UserResponse>> AddAsync(CreateUserRequest request);
    Task<Result> UpdateAsync(string userId, UpdateUserRequest request);
    Task<Result> ToggleStatusAsync(string userId);
    Task<Result> UnlockAsync(string userId);
}
