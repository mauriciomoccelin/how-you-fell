namespace HowYouFell.Api.Models;

public class Person : Entity
{
    public string Email { get; private set; } = null!;
    public ICollection<PersonFelling> Fellings { get; private set; }

    private Person()
    {
        Fellings = Enumerable.Empty<PersonFelling>().ToList();
    }

    public class Factory
    {
        public static Person Create(string email)
        {
            return new Person
            {
                Email = email.ToLower()
            };
        }
    }
}
