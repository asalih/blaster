using Blaster.Infrastructure.Entity;
using Blaster.Infrastructure.Utility;
using Blaster.Infrastructure.Utility.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blaster.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddBlasterPgSqlDataContext(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(Configuration["Database:ConnString"]);
            });

            return services;
        }

        static readonly string DbName = Guid.NewGuid() + ".db";
        public static IServiceCollection AddSqliteDataContext(this IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite("Filename=App_Data/data.db");
            });

            return services;
        }

        public static IServiceCollection AddBlasterIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>(v =>
            {
                v.SignIn.RequireConfirmedAccount = false;
                v.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            services.AddTransient<IEmailHelper, EmailHelper>();
            services.AddTransient<CustomUrlHelper>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                //options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            return services;
        }
    }
}
