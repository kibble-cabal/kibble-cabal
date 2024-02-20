using Godot;

namespace KibbleCabal.Core.Pet
{
    [GlobalClass]
    public partial class RenameContextAction : RPetContextAction
    {
        public static readonly StringName MenuIdentifier = "pet/interact";
        public static readonly string[] RandomNames = [
            "Fluffy",
            "Buzz",
            "Fido",
            "Princess"
        ];

        protected override string _GetDisplayText(Context? ctx)
        {
            if (ctx is not null && ctx.Pet.Name.Length > 0)
                return $"Rename {ctx.Pet.Name}...";
            return "Rename...";
        }

        protected override StringName[] _GetMenuIdentifiers() => [MenuIdentifier];

        protected override void _OnPress(Context? ctx)
        {
            if (ctx is null) return;
            ctx.Pet.Name = RandomNames[GD.RandRange(0, RandomNames.Length - 1)];
            GD.Print($"Renamed to {ctx.Pet.Name}");
        }
    }
}