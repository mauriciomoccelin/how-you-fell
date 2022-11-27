using System.Collections.ObjectModel;
using System.Security.Claims;

namespace HowYouFell.Test.Unit;

public class AspNetUserServiceTestFixture : IDisposable
{
    public AutoMocker Mocker { get; set; }
    public string UserEmail => "admin@mail.com";

    public AspNetUserServiceTestFixture()
    {
        Mocker = new AutoMocker();
    }

    public void Dispose()
    {
    }

    public IAspNetUserService GenereteAspNetUser()
    {
        Mocker = new AutoMocker();

        var service = Mocker.CreateInstance<AspNetUserService>();
        return service;
    }

    public IConfigurationSection GenereteConfigurationSection(bool withEmail = false)
    {
        var configurationSection = new Mock<IConfigurationSection>();

        configurationSection
            .SetupGet(x => x.Value)
            .Returns(!withEmail ? string.Empty : UserEmail);
        
        configurationSection
            .Setup(s => s.GetChildren())
            .Returns(new List<IConfigurationSection> { configurationSection.Object });

        return configurationSection.Object;
    }

    public HttpContext GenereteHttpContext(bool withNeededClaims = false)
    {
        var context = new DefaultHttpContext();
        var identity = GenereteIdentity();
        var claims = GenereteClaims();

        if(withNeededClaims)
        {
            var emailCLaim = new Claim(ClaimTypes.Email, UserEmail);
            claims.Add(emailCLaim);
        }
        
        identity.AddClaims(claims);

        context.User = new ClaimsPrincipal(identity);

        return context;
    }

    public ClaimsIdentity GenereteIdentity()
    {
        var identity = new ClaimsIdentity();
        return identity;
    }

    public ICollection<Claim> GenereteClaims()
    {
        var claims = new Collection<Claim>();
        return claims;
    }
}
