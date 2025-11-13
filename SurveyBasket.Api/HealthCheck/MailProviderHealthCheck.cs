using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SurveyBasket.Api.Settings;

namespace SurveyBasket.Api.HealthCheck;

public class MailProviderHealthCheck(IOptions<MailSettings> mailSettings) : IHealthCheck
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
		try
		{
			using var smtp = new SmtpClient();

			await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port,SecureSocketOptions.StartTls);

			await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);

			return HealthCheckResult.Healthy();
		}
		catch (Exception ex)
		{

			return HealthCheckResult.Unhealthy(exception:ex);
		}
    }
}
