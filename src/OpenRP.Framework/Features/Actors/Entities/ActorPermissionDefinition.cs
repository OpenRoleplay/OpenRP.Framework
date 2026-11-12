namespace OpenRP.Framework.Features.Actors.Entities
{
    /// <summary>
    /// A permission a feature offers for actors, such as "bank.deposit". Register one with
    /// AddActorPermission(name, description); /editactors lists them all.
    /// </summary>
    public sealed record ActorPermissionDefinition(string Name, string Description);
}
