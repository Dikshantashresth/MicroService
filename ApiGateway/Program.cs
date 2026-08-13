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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("GatewayJwt", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = tokenvalidationparameters;

    });
builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
await app.UseOcelot();

app.Run();
