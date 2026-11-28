using FluentValidation;
using Hoofd;
using Hoofd.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o => o.Conventions.Add(new RoutePrixConvention("api")));
builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<Horse>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
   options.Cookie.Name = "hoofd.auth";
   options.Cookie.HttpOnly = true;
   options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
   options.Cookie.SameSite = SameSiteMode.Lax;
   options.ExpireTimeSpan = TimeSpan.FromDays(2);
   options.SlidingExpiration = true;

   // Return 401/403 instead of redirect.
    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins("http://localhost:5173")
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()
));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();