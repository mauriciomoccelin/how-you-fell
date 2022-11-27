namespace HowYouFell.Api.Models;

public class TenantEquip : Entity
{
    public string? Description { get; private set; }
    public ICollection<string> AllowEmails { get; set; }

    private TenantEquip()
    {
        AllowEmails = Enumerable.Empty<string>().ToList();
    }

    public class Factory
    {
        public static TenantEquip Create(string description)
        {
            return new TenantEquip
            {
                Description = description
            };
        }
    }
}
