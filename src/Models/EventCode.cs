namespace HowYouFell.Api.Models;

public class EventCode
{
    public static EventId Ok => new(200, nameof(Ok));
    public static EventId Created => new(201, nameof(Created));
    public static EventId NotFound => new(404, nameof(NotFound));
    public static EventId Forbiden => new(403, nameof(Forbiden));
    public static EventId Unauthorized => new(401, nameof(Unauthorized));
}
