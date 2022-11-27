namespace HowYouFell.Api.Services;

public interface IAspNetUserService
{
    bool HasUserEmail();
    string GetUserEmail();
    bool CanRegisterTenant();
}
