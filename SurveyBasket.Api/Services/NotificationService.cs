using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Api.Helpers;

namespace SurveyBasket.Api.Services;

public class NotificationService(ApplicationDbContext context,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task SendNewPollNotification(int? pollId)
    {
        IEnumerable<Poll> polls = [];

        if (pollId.HasValue)
        {
            var poll = await _context.Polls.Where(x => x.Id == pollId && x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            polls = [poll!];
        }
        else
        {
            polls = await _context.Polls.Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking().ToListAsync();
        }

        //TODO: Get All Users in Member Role 
        var users = await _userManager.Users.ToListAsync();

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        foreach(var poll in polls)
        {
            foreach(var user in users)
            {
                var placeHolders = new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName },
                    {"{{pollTill}}",poll.Title },
                    {"{{endDate}}",poll.EndsAt.ToString() },
                    {"{{url}}",$"{origin}/polls/start/{poll.Id}" }
                };
                var emailBody = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeHolders);
                await _emailSender.SendEmailAsync(user.Email!, $"Survey Basket: New Poll - {poll.Title}", emailBody);
            }
        }
    }
}
