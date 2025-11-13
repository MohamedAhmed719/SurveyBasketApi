using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager,ApplicationDbContext context) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId,CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.Where(x=>x.Id == userId).ProjectToType<UserProfileResponse>().FirstOrDefaultAsync(cancellationToken);

        return Result.Success(user!);
    }

    public async Task<Result> UpdateProfileAsync(string userId,UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        //user = request.Adapt(user);

        await _userManager.Users.Where(x=>x.Id == userId).ExecuteUpdateAsync(setter =>

            setter
            .SetProperty(x => x.FirstName, request.FirstName)
            .SetProperty(x => x.LastName, request.LastName)
        );

        await _userManager.UpdateAsync(user!);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId,ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            return Result.Success();
        }


        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await (from u in _context.Users
                           join ur in _context.UserRoles
                           on u.Id equals ur.UserId
                           join r in _context.Roles on ur.RoleId equals r.Id into roles
                           where !roles.Any(x => x.Name == DefaultRoles.Member)
                           select new
                           {
                               u.Id,
                               u.FirstName,
                               u.LastName,
                               u.Email,
                               u.IsDisabled,
                               Roles = roles.Select(x => x.Name)
                           }
                           )
                           .GroupBy(u => new
                           {
                               u.Id,
                               u.FirstName,
                               u.LastName,
                               u.Email,
                               u.IsDisabled
                           }
                           )
                           .Select(u => new UserResponse(
                               u.Key.Id,
                               u.Key.FirstName,
                               u.Key.LastName,
                               u.Key.Email,
                               u.Key.IsDisabled,
                               u.SelectMany(x => x.Roles).ToList()
                               )
                           ).ToListAsync();
                           
                           

        return users;
    }


    public async Task<Result<UserResponse>> GetAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var userRoles = await _userManager.GetRolesAsync(user);

        var response = new UserResponse(user.Id, user.FirstName, user.LastName, user.Email!, user.IsDisabled, userRoles);

        return Result.Success(response);
    }

    public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request)
    {
        var isEmailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email);

        if (isEmailExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var allowedRoles = await _context.Roles.Select(x => x.Name).ToListAsync();

        if (request.Roles.Except(allowedRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);

            var response = new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.IsDisabled, request.Roles);

            return Result.Success(response);
        }

        var error = result.Errors.First();

        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string userId,UpdateUserRequest request)
    {
        var isEmailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != userId);

        if (isEmailExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var allowedRoles = await _context.Roles.Select(x => x.Name).ToListAsync();

        if (request.Roles.Except(allowedRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        user = request.Adapt(user);

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {

            await _context.UserRoles.Where(x => x.UserId == userId)
                .ExecuteDeleteAsync();

             await _userManager.AddToRolesAsync(user,request.Roles);
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ToggleStatusAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user.IsDisabled = !user.IsDisabled;

        await _userManager.UpdateAsync(user);

        return Result.Success();
        
    }

    public async Task<Result> UnlockAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        await _userManager.SetLockoutEndDateAsync(user, null);

        return Result.Success();

    }
}
