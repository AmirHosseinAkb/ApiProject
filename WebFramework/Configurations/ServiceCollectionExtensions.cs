using System.Net;
using System.Security.Claims;
using System.Text;
using Common;
using Common.Exceptions;
using Data.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace WebFramework.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services,SiteSettings siteSettings)
    {
        var jwtSettings = siteSettings.JwtSettings;
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            var decryptKey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.EncryptKey);
            var validationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                RequireSignedTokens = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateIssuer = true,
                ValidAudience = jwtSettings.Audience,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                TokenDecryptionKey = new SymmetricSecurityKey(decryptKey)
            };
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = validationParameters;
            options.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception != null)
                        throw new UnauthorizedAccessException("Authentication Failed");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    if (context.AuthenticateFailure != null)
                        throw new UnauthorizedAccessException("Authentication Failure");
                    throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication Failure",context.AuthenticateFailure);
                },
                OnTokenValidated = async context =>
                {
                    var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                    var userClaims = context.HttpContext.User.Claims.ToList();
                    if(!userClaims.Any())
                        context.Fail("This token has no claims");

                    var securityStamp =
                        userClaims.FirstOrDefault(c => c.Type == new ClaimsIdentityOptions().SecurityStampClaimType);

                    if(securityStamp == null)
                        context.Fail("This token has no security stamp");

                    var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    if(userId==null)
                        context.Fail("This token has no user id field");

                    var user = await userRepository.GetByIdAsync(context.HttpContext.RequestAborted,int.Parse(userId!.Value));
                    if(user.SecurityStamp!=Guid.Parse(securityStamp!.Value))
                        context.Fail("Token Security Stamp Is Not Valid");
                    if (!user.IsActive)
                        context.Fail("User is not active");

                    await userRepository.UpdateLastLoginDateAsync(user, context.HttpContext.RequestAborted);
                }
            };
        });
    }
    
}