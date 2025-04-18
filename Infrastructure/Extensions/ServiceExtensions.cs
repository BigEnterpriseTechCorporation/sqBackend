using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Database
        //services.AddDbContext<AppDbContext>(options =>
         //   options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        
        // Services
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IUnitService, UnitService>();
        services.AddTransient<IExerciseService, ExerciseService>();
        
        // External services
        //services.AddScoped<IEmailService, SendGridEmailService>();
        
        // Identity
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        
        return services;
    }
}