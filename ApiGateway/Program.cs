using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("JWTSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("CRITICAL: The system failed to read 'JwtSettings:SecretKey' from appsettings.json. Verify your JSON nesting structure.");
}

var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

var tokenvalidationparameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingkey,
    ValidateIssuer = true,
    ValidIssuer = issuer,
    ValidateAudience = true,
    ValidAudience = audience,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};



builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = tokenvalidationparameters;
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"[JWT Auth Failed] Exception: {context.Exception.GetType().Name}");
                logger.LogError($"[JWT Auth Failed] Message: {context.Exception.Message}");

                if (context.Exception is SecurityTokenExpiredException)
                {
                    logger.LogError("[JWT Auth Failed] Token has expired");
                }
                else if (context.Exception is SecurityTokenInvalidSignatureException)
                {
                    logger.LogError("[JWT Auth Failed] Token signature is invalid - SecretKey mismatch?");
                }
                else if (context.Exception is SecurityTokenInvalidIssuerException)
                {
                    logger.LogError($"[JWT Auth Failed] Invalid issuer. Expected: {issuer}");
                }
                else if (context.Exception is SecurityTokenInvalidAudienceException)
                {
                    logger.LogError($"[JWT Auth Failed] Invalid audience. Expected: {audience}");
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var principal = context.Principal;
                logger.LogInformation("[JWT Token Valid] User authenticated successfully");
                logger.LogInformation($"[JWT Token Valid] Claims: {string.Join(", ", principal?.Claims.Select(c => $"{c.Type}={c.Value}") ?? new[] { "N/A" })}");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    logger.LogWarning("[JWT Message Received] No Authorization header found in request");
                }
                else
                {
                    logger.LogInformation("[JWT Message Received] Authorization header present, format correct");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
await app.UseOcelot();

app.Run();
