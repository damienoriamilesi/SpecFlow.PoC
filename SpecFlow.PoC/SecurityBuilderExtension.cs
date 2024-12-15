using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace SpecFlow.PoC;

/// <summary>
/// Startup extension for :
/// - Security - token validation
/// - Database - EF Core
/// </summary>
public static class SecurityBuilderExtension
{
    /// <summary>
    /// EF Core registration
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void AddEntityFramework(this IServiceProvider serviceProvider)
    {
        // Ensure database is created during application startup
        using var scope = serviceProvider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
        if (dbContext.Employees.Any()) return;
        dbContext.Employees.AddRange(TestFixture.BuildEmployees());
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Token validation
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Task ValidateToken(MessageReceivedContext context)
    {
        try
        {
            //context.Token = GetToken(context.Request);
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(context.Token, context.Options.TokenValidationParameters, out var validatedToken);

            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            context.Principal = new ClaimsPrincipal();

            var claimsIdentity = new ClaimsIdentity(jwtSecurityToken!.Claims.ToList(),
                "JwtBearerToken", ClaimTypes.NameIdentifier, ClaimTypes.Role);
            context.Principal.AddIdentity(claimsIdentity);

            context.Success();

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            context.Fail(e);
        }

        return Task.CompletedTask;
    }
}