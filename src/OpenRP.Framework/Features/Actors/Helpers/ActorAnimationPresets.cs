namespace OpenRP.Framework.Features.Actors.Helpers
{
    /// <summary>
    /// Common looping animations for actors, so admins can pick one instead of looking up library and animation names.
    /// </summary>
    public static class ActorAnimationPresets
    {
        public record Preset(string Description, string Library, string Name);

        public static readonly IReadOnlyList<Preset> All = new List<Preset>
        {
            new Preset("Sit on the ground", "BEACH", "ParkSit_M_loop"),
            new Preset("Lean on a wall", "GANGS", "leanIDLE"),
            new Preset("Smoke against a wall", "SMOKING", "M_smklean_loop"),
            new Preset("Look around", "COP_AMBIENT", "Coplook_loop"),
            new Preset("Injured against a wall", "SWAT", "gnstwall_injurd"),
            new Preset("Sit at a table", "INT_OFFICE", "OFF_Sit_Idle_Loop"),
            new Preset("Lean on a bar", "BAR", "Barcustom_loop"),
            new Preset("Chat", "PED", "IDLE_chat"),
            new Preset("Dealer", "DEALER", "DEALER_IDLE"),
            new Preset("Wash up", "INT_HOUSE", "wash_up"),
        };
    }
}
