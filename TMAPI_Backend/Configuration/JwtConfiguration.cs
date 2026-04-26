using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TMAPI_Backend.Configuration
{
    public static class JwtConfiguration
    {
        public static void AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is missing.");

            string jwtIssuer = configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT issuer is missing.");

            string jwtAudience = configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT audience is missing.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            services.AddAuthorization();
        }
    }
}