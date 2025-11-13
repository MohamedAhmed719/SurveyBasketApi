using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.Api.Extensions;
using SurveyBasket.Api.HealthCheck;
using SurveyBasket.Api.Settings;
using SurveyBasket.OpenApiTransformers;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveyBasket.Api;

public static class Dependencies
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
    {

        services.AddControllers();

        var conncetionString = configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("conncetion string 'DefaultConnection' not found!");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conncetionString));

        services.AddMapsterConfig().AddFluentValidationConfig();

        services.AddHealthChecksConfig(configuration);

        services.AddRateLimitingConfig();

        services.AddAuthConfig(configuration);
        services.AddBackgrundJobsConfig(configuration);
        services.AddRedisConfig(configuration);
        services.AddApiVersioningConfig();
        services.AddOpenApiConfig();

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<ICacheService, CacheService>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();


        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();

        return services;    
    }

    public static IServiceCollection AddApiVersioningConfig(this IServiceCollection service)
    {

        service.AddApiVersioning(options => {
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        // instal asp.Versiniong.Http
        // Asp.Versiniong.mvc.ApiExplorer
        return service;
    }
    public static IServiceCollection AddOpenApiConfig(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var apiVersionDescriptionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            services.AddOpenApi(description.GroupName, options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                options.AddDocumentTransformer(new ApiVersioningTransformer(description));
            });
        }

        return services;
    }

    public static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {

        var mappingConfiguration = TypeAdapterConfig.GlobalSettings;

        mappingConfiguration.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfiguration));
        return services;
    }

    public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation().AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection AddAuthConfig(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.Key))
                };
            });

        services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }
    public static IServiceCollection AddBackgrundJobsConfig(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();
        return services;
    }

    public static IServiceCollection AddRedisConfig(this IServiceCollection services,IConfiguration configuration)
    {

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetValue<string>("ConnectionStrings:Redis");
            options.InstanceName = "SurveyBasket";
        });
        return services;
    }

    public static IServiceCollection AddHealthChecksConfig(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")??
            throw new InvalidOperationException("Invalid connection string 'DefaultConnection'");

        services.AddHealthChecks().AddSqlServer(connectionString)
            .AddHangfire(x => { x.MinimumAvailableServers = 1; })
            .AddCheck<MailProviderHealthCheck>(name:"mail provider");
            

        // to know what causes problem you need to install 'healthchecks.ui.client' 

        return services;
    }

    public static IServiceCollection AddRateLimitingConfig(this IServiceCollection services)
    {

        services.AddRateLimiter(rateLimit =>
        {

            rateLimit.AddPolicy("ipLimit", httpContext =>
                  RateLimitPartition.GetFixedWindowLimiter(
                      partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                      factory => new FixedWindowRateLimiterOptions
                      {
                          PermitLimit = 2,
                          Window = TimeSpan.FromSeconds(20)
                      }
                      )
            );

            rateLimit.AddPolicy("userLimit", httpContext =>
                 RateLimitPartition.GetFixedWindowLimiter(
                     partitionKey: httpContext.User.GetUserId(),
                     factory => new FixedWindowRateLimiterOptions
                     {
                         PermitLimit = 2,
                         Window = TimeSpan.FromSeconds(20)
                     }
                     )
            );

            rateLimit.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimit.AddConcurrencyLimiter("concurrency", builder =>
            {
                builder.PermitLimit = 3;
                builder.QueueLimit = 1;
                builder.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });


            //rateLimit.AddTokenBucketLimiter("token", options =>
            //{
            //    options.TokenLimit = 3;
            //    options.QueueLimit = 1;
            //    options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
            //    options.AutoReplenishment = true;
            //    options.TokensPerPeriod = 2;
            //});

            //rateLimit.AddFixedWindowLimiter("fixed", options =>
            //{
            //    options.PermitLimit = 3;
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //    options.Window = TimeSpan.FromSeconds(30);
            //});

        });

        return services;
    }

    
}
