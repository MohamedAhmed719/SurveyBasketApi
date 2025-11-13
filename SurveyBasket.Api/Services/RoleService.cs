using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager,ApplicationDbContext context): IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken=default, bool? IncludeDisabled = false)
    {
        var roles = await _roleManager.Roles.
            Where(x => !x.IsDefault && (!IncludeDisabled.HasValue || IncludeDisabled.Value))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);
        return roles;
    }

    public async Task<Result<RoleDetailResponse>> GetAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailResponse
        (
            role.Id,
            role.Name!,
            role.IsDeleted,
            permissions.Select(x=>x.Value)
        );

        return Result.Success(response);
    }

    public async Task<Result<RoleDetailResponse>> AddAsync(CreateRoleRequest request)
    {
        var isRoleExists = await _roleManager.RoleExistsAsync(request.Name);

        if (isRoleExists)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRoleName);

        var allowedPermssions = DefaultPermissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermssions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermission);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            var permissions = request.Permissions.Select(x => new IdentityRoleClaim<string>
            {
                ClaimType = DefaultPermissions.Type,
                ClaimValue = x,
                RoleId = role.Id
            }).ToList();

            await _context.AddRangeAsync(permissions);
            await _context.SaveChangesAsync();

            var response = new RoleDetailResponse
            (
                role.Id,
                role.Name!,
                role.IsDeleted,
                request.Permissions
            );

            return Result.Success(response);
        }

        var error = result.Errors.First();

        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string id,CreateRoleRequest request)
    {
        var isRoleExists = await _roleManager.Roles.Where(x => x.Name == request.Name && x.Id != id).AnyAsync();

        if (isRoleExists)
            return Result.Failure(RoleErrors.DuplicatedRoleName);

        var allowedPermssions = DefaultPermissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermssions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermission);

        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure(RoleErrors.RoleNotFound);

        role.Name = request.Name;

        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            var CurrentPermissions = await _context.RoleClaims.Where(x => x.RoleId == id).Select(x => x.ClaimValue).ToListAsync();

            var newPermissions = request.Permissions.Except(CurrentPermissions).Select(x=> new IdentityRoleClaim<string>
            {
                ClaimType = DefaultPermissions.Type,
                ClaimValue = x,
                RoleId = role.Id
            })
                .ToList();

            var removedPermissions = CurrentPermissions.Except(request.Permissions);

            await _context.RoleClaims.Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();

            await _context.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure(RoleErrors.RoleNotFound);

        role.IsDeleted = !role.IsDeleted;

        var result = await _roleManager.UpdateAsync(role);


        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
