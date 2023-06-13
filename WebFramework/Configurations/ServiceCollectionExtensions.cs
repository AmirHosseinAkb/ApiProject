using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace WebFramework.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var secretKey = Encoding.UTF8.GetBytes("AmirHossein200");
            var validationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                RequireSignedTokens = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuerSigningKey = true,
                ValidIssuer = "Licensify.ir",
                ValidateIssuer = true,
                ValidAudience = "Licensify.ir",
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateLifetime = true
            };
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = validationParameters;
        });
    }
    
}