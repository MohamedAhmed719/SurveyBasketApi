using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.WebEncoders;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Crypto;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    SignInManager<ApplicationUser> signInManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender,
    ApplicationDbContext context) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ApplicationDbContext _context = context;
    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> GetJwtAsync(string email,string password)
    {
        
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, true);

        if (signInResult.Succeeded)
        {
            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user);

            var (token, expiresIn) = _jwtProvider.GenerateJwtToken(user, userRoles, userPermissions);

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);
            var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email!, token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }

        var error = signInResult.IsNotAllowed ? UserErrors.EmailNotConfirmed : signInResult.IsLockedOut ? UserErrors.UserLockedOut : UserErrors.InvalidCredentials;

        return Result.Failure<AuthResponse>(error);
    }

    public async Task<AuthResponse?> GetRefreshTokenAsync(string token,string refreshToken)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return null;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return null;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user);
        var (newToken, expiresIn) = _jwtProvider.GenerateJwtToken(user,userRoles,userPermissions);

        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);
        var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return response;
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return false;

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var isEmailExists = await _userManager.FindByEmailAsync(request.Email);

        if (isEmailExists is not null)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        
        var user = request.Adapt<ApplicationUser>();

        user.UserName = user.Email;

        var result = await _userManager.CreateAsync(user,request.Password);

        if (result.Succeeded)
        {

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation Code: {code}", code);
            await SendConfirmationEmail(user, code);

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedEmailConfirmed);


        var code = request.Code;
        try
        {
             code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ResendConfirmEmailCodeAsync(string email, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedEmailConfirmed);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation Code: {code}", code);
        await SendConfirmationEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Success();

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

        _logger.LogInformation("Reset Code:{code}", code);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        await SendResetPasswordEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult identityResult;

        try
        {
           var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

            identityResult = await _userManager.ResetPasswordAsync(user, request.Code, request.NewPassword); 
        }
        catch (FormatException)
        {
            identityResult = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if (identityResult.Succeeded)
            return Result.Success();

        var error = identityResult.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    private async Task SendConfirmationEmail(ApplicationUser user,string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
        {
            {"{{name}}",user.FirstName},
            {"{{action_url}}",$"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
        });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey-Basket: Email Confirmation", emailBody));

        await Task.CompletedTask;
    }

    public async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword", new Dictionary<string, string>
        {
            {"{{name}}",user.FirstName},
            {"{{action_url}}",$"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
        });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey-Basket: Change Password", emailBody));

        await Task.CompletedTask;
    }
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

   public async Task<(IEnumerable<string> roles,IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var permissions = await (from r in _context.Roles
                                 join p in _context.RoleClaims
                                 on r.Id equals p.RoleId
                                 where userRoles.Contains(r.Name!)
                                 select p.ClaimValue
                                 )
                                 .Distinct()
                                 .ToListAsync();

        return(userRoles, permissions!);
    }
}
