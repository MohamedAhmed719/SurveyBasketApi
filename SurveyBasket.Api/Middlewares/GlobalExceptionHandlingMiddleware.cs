
namespace SurveyBasket.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{

			_logger.LogError(ex, "Something went wrong :{message}", ex.Message);

			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "Internal Server Error",
				Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            };

			context.Response.StatusCode = StatusCodes.Status500InternalServerError;

			await context.Response.WriteAsJsonAsync(problemDetails);
		}
    }
}
