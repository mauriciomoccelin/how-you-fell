namespace HowYouFell.Api.Models;

public class TenantThread : Entity
{
    public string? Description { get; private set; }

    private TenantThread()
    {
    }

    public class Factory
    {
        public static TenantThread Create(string description)
        {
            return new TenantThread
            {
                Description = description
            };
        }
    }
}
