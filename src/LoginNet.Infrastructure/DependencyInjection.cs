using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using LoginNet.Infrastructure.Persistence;
using LoginNet.Infrastructure.Persistence.Repositories;
using LoginNet.Domain.Interfaces;
using LoginNet.Application.Interfaces;
using LoginNet.Infrastructure.Security;
using LoginNet.Infrastructure.Services;
using LoginNet.Application.Common.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LoginNet.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext with Npgsql provider
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Configure Data Protection to persist keys in the database
            services.AddDataProtection()
                .PersistKeysToDbContext<AppDbContext>();

            // JWT Auth configuration
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["Jwt:Key"]!))
                    };
                });
            services.AddAuthorization();

            // Register Repositories
            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IRoleRepository, EfRoleRepository>();
            services.AddScoped<INoteRepository, EfNoteRepository>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // Register Infrastructure Security Services
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, JwtTokenService>();

            // Register Mediator Implementation
            services.AddScoped<IMediator, Mediator>();

            return services;
        }
    }
}
