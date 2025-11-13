using SurveyBasket.Api;
using Serilog;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using SurveyBasket.Api.Abstractions.Consts;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddDistributedMemoryCache();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
    [
       new HangfireCustomBasicAuthenticationFilter
       {
           User = app.Configuration.GetValue<string>("HangfireSettings:UserName"),
           Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
       }
    ],
    DashboardTitle = "Survey Basket Jobs",
   // IsReadOnlyFunc = (DashboardContext context) => true
});

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();

var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollNotification", () => notificationService.SendNewPollNotification(null), Cron.Daily);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
   
});

app.Run();
