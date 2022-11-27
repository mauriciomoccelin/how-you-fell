using System.Security.Claims;
using MongoDB.Driver;

namespace HowYouFell.Api.Services;

public class AspNetUserService : IAspNetUserService
{
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AspNetUserService(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor
    )
    {
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
    }

    private Microsoft.AspNetCore.Http.HttpContext GetHttpContent()
    {
        var context = httpContextAccessor?.HttpContext;

        if (context is null)
        {
            throw new ArgumentNullException();
        }

        return context;
    }

    public bool HasUserEmail()
    {
        var hasEmail = GetHttpContent().User.Claims.Any(
            c => c.Type == ClaimTypes.Email
        );

        return hasEmail;
    }

    public string GetUserEmail()
    {
        var claim = GetHttpContent().User.Claims.FirstOrDefault(
            c => c.Type == ClaimTypes.Email
        );

        return claim?.Value ?? string.Empty;
    }

    public bool CanRegisterTenant()
    {
        const string section = "App:AllowEmailsCreateTenant";

        return configuration
            .GetSection(section)
            .Get<string[]>()
            .Contains(GetUserEmail()); 
    }
}
