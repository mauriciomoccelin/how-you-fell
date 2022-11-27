namespace HowYouFell.Api.Models;

public class Tenant : Entity
{
    public string? Description { get; private set; }
    public ICollection<TenantEquip> Equips { get; private set; }
    public ICollection<TenantThread> Threads { get; private set; }

    private Tenant()
    {
        Equips = Enumerable.Empty<TenantEquip>().ToList();
        Threads = Enumerable.Empty<TenantThread>().ToList();
    }

    public class Factory
    {
        public static Tenant Create(string description)
        {
            return new Tenant
            {
                Description = description
            };
        }
    }
}
