namespace OpenRP.Framework.Features.ActorRoleplay.Entities
{
    /// <summary>
    /// An actor's reply: what it says and does in chat, and the actions it wants to perform.
    /// </summary>
    public sealed record ActorReply(List<ActorRoleplayLine> Lines, List<ActorActionRequest> Actions);

    /// <summary>
    /// A "/action name rest" line: the rest holds the player's name and maybe a detail, which the service separates
    /// using the names of the players the actor heard.
    /// </summary>
    public sealed record ActorActionRequest(string Name, string Rest)
    {
        public string GetCommandText()
        {
            return $"/action {Name} {Rest}";
        }
    }

    /// <summary>
    /// A speaker the actor heard recently, so an action can find the player it's for.
    /// </summary>
    public sealed record ActorSpeaker(SampSharp.Entities.EntityId Player, string Name, DateTime HeardAt);
}
