using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using TaskService.Data;
using TaskService.Repository;
using TaskService.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddScoped<ITaskService, TaskServices>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUnitofWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();

// Add JWT authentication configuration
var jwtSettings = builder.Configuration.GetSection("JWTSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("CRITICAL: JWTSettings:SecretKey not found in TaskService appsettings.json");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
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
        options.TokenValidationParameters = tokenValidationParameters;
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"[TaskService JWT Auth Failed] {context.Exception.GetType().Name}: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("[TaskService JWT Token Valid] User authenticated");
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"[TaskService Request] Incoming request to: {context.Request.Path}");
    logger.LogInformation($"[TaskService Headers] Authorization: {context.Request.Headers["Authorization"].FirstOrDefault() ?? "MISSING"}");
    logger.LogInformation($"[TaskService Headers] X-User-Id: {context.Request.Headers["X-User-Id"].FirstOrDefault() ?? "MISSING"}");
    logger.LogInformation($"[TaskService Headers] X-User-Email: {context.Request.Headers["X-User-Email"].FirstOrDefault() ?? "MISSING"}");

    logger.LogInformation("[TaskService Headers] All incoming headers:");
    foreach (var header in context.Request.Headers)
    {
        logger.LogInformation($"  - {header.Key}: {header.Value}");
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();