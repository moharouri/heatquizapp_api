using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HeatQuizAPI.Extensions
{
    public static class ServiceExtensions
    {
        //Must be saved to secure location
        private readonly static string secretKey = "mysupersecret_secretkey!12345678";

        private readonly static SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        private static IConfigurationRoot configuration =   new ConfigurationBuilder()
                                                            .SetBasePath(Directory.GetCurrentDirectory())
                                                            .AddJsonFile("appsettings.json")
                                                            .Build();

        //configure CORS Policy 
        public static void ConfigureCors(this IServiceCollection services) =>
         services.AddCors(options =>
         {
             options.AddPolicy("CorsPolicy", builder =>
             builder.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
        });

        //configzure IIS 
        public static void ConfigureIISIntegration(this IServiceCollection services) =>
         services.Configure<IISOptions>(options =>
         {
        });


        //configure PostgreSQL
        public static void ConfigureDatabaseContext(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>((provider, options) =>
               options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")),
               ServiceLifetime.Transient, ServiceLifetime.Transient);
        }

        //configure authorization
        public static void ConfigureAuthenticationAndAuthorization(this IServiceCollection services)
        {
            //Get parameters
            var issure = configuration["JwtSettings:Issure"]; 
            var audience = configuration["JwtSettings:Audience"];

            services.AddIdentity<User, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequiredLength = 0;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;

            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            var tokenValidationParameters = new TokenValidationParameters
            {
                //The signing key must match
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                //Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = issure,

                //validate the JWT Audience (aud) claim

                ValidateAudience = true,
                ValidAudience = audience,

                //validate the token expiry
                ValidateLifetime = true,

                // If you  want to allow a certain amout of clock drift
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });


            services.AddAuthorization(options =>
            {
                //Add roles "admin" and "course_editor"
                options.AddPolicy("admin",
                    authBuilder =>
                    {
                        authBuilder.RequireRole("admin");
                    });

                options.AddPolicy("course_editor",
                    authBuilder =>
                    {
                        authBuilder.RequireRole("course_editor");
                    });
            });

            services.AddScoped<UserManager<User>>();

        }

    }
}
