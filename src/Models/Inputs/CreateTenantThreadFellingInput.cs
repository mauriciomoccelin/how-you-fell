namespace HowYouFell.Api.Models.Inputs;

public class CreateTenantThreadFellingInput
{
    public string TeamId { get; set; } = null!;
    public string ThreadId { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string Description { get; set; } = null!;
    public PersonFellingType Type { get; set; }
}