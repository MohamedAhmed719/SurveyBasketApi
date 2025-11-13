using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken=default, bool? IncludeDisabled=false);
    Task<Result<RoleDetailResponse>> GetAsync(string id);
    Task<Result<RoleDetailResponse>> AddAsync(CreateRoleRequest request);
    Task<Result> UpdateAsync(string id, CreateRoleRequest request);
    Task<Result> ToggleStatusAsync(string id);
}
