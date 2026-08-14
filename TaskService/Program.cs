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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TrustedGatewayOnly", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var httpContext = builder.Services.BuildServiceProvider()
                .GetRequiredService<IHttpContextAccessor>().HttpContext;

            if (httpContext == null) return false;

            // 1. Ensure the user identity header injected by Ocelot exists
            var hasUserHeader = httpContext.Request.Headers.TryGetValue("X-User-Id", out var userId);

            return hasUserHeader && !string.IsNullOrWhiteSpace(userId);
        });
    });
});
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();