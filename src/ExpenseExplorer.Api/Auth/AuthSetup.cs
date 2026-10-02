using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace ExpenseExplorer.Api.Auth;

internal static class AuthSetup
{
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        IConfigurationSection section = builder.Configuration.GetSection(AuthOptions.Section);
        builder.Services.Configure<AuthOptions>(section);
        builder.Services.AddSingleton(services =>
            SigningKey.LoadOrCreate(services.GetRequiredService<IOptions<AuthOptions>>().Value.SigningKeyPath));
        builder.Services.AddSingleton<SessionIssuer>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.RequireHttpsMetadata = false;
        });
        builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<AuthOptions>, SigningKey, TimeProvider>((jwt, auth, key, clock) =>
                jwt.TokenValidationParameters = AccessTokens.ValidationParameters(auth.Value, key, clock));

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CanRead, policy => policy.RequireRole(Policies.Readers))
            .AddPolicy(Policies.CanEdit, policy => policy.RequireRole(Policies.Editors))
            .AddPolicy(Policies.CanViewLogs, policy => policy.RequireRole(Policies.Admins));

        int attemptsPerMinute = section.Get<AuthOptions>()?.SignInAttemptsPerMinute ?? new AuthOptions().SignInAttemptsPerMinute;
        builder.Services.AddRateLimiter(limiter =>
        {
            limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            limiter.AddPolicy(AuthEndpoints.SignInRateLimit, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = attemptsPerMinute, Window = TimeSpan.FromMinutes(1) }));
        });

        return builder;
    }
}
