using Godot;

namespace KibbleCabal.Core.Pet
{
    [GlobalClass]
    public partial class RenameContextAction : Resource, IPetContextAction
    {
        public static readonly StringName MenuIdentifier = "pet/interact";
        public static readonly string[] RandomNames = [
            "Fluffy",
            "Buzz",
            "Fido",
            "Princess"
        ];

        string IContextAction<IPetContextAction.Context>._GetDisplayText(IPetContextAction.Context ctx)
        {
            if (ctx.Pet.Name.Length > 0) return $"Rename {ctx.Pet.Name}...";
            return "Rename...";
        }

        StringName[] IContextAction._GetMenuIdentifiers() => [MenuIdentifier];

        void IContextAction<IPetContextAction.Context>._OnPress(IPetContextAction.Context ctx)
        {
            ctx.Pet.Name = RandomNames[GD.RandRange(0, RandomNames.Length - 1)];
            GD.Print($"Renamed to {ctx.Pet.Name}");
        }
    }
}