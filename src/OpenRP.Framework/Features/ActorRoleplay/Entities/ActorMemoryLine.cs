namespace OpenRP.Framework.Features.ActorRoleplay.Entities
{
    /// <summary>
    /// A line an actor remembers: something it saw or heard nearby, or something it said or did itself.
    /// </summary>
    public record ActorMemoryLine(bool IsOwnLine, string Text);
}
