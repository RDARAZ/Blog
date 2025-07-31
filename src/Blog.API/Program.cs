using Blog.API.Configuration;
using Blog.Application.Behaviors;
using Blog.Application.Features.Authentication.Commands.RegisterUser;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Data.DataSeeding;
using Blog.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwtAuth();

//Add DbContext
builder.Services.AddDbContext<BlogDbContext>(options => 
   options.UseSqlServer(
       builder.Configuration.GetConnectionString("DefaultConnection"),
       b => b.MigrationsAssembly("Blog.Infrastructure")
    ));

builder.Services.AddIdentity<User, Role>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<BlogDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(RegisterUserCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        await context.Database.MigrateAsync();
        await IdentityDataSeeder.SeedAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        logger?.LogError(ex, "An error occurred during migration or seeding.");
        if(app.Environment.IsDevelopment())
        {
            throw;
        }
    }

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();