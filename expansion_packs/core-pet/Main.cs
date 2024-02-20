
using System.Collections.Generic;
using Godot;

namespace KibbleCabal.Core.Pet
{
    public partial class Main : GodotObject
    {
        public static readonly IEnumerable<RContextAction> Actions = [
            GD.Load<RContextAction>("res://expansion_packs/core-pet/action/resources/FulfillActivity.instruction.tres"),
            GD.Load<RContextAction>("res://expansion_packs/core-pet/action/resources/FulfillEnergy.instruction.tres"),
            GD.Load<RContextAction>("res://expansion_packs/core-pet/action/resources/FulfillHunger.instruction.tres"),
            GD.Load<RContextAction>("res://expansion_packs/core-pet/action/resources/FulfillThirst.instruction.tres"),
        ];

        public static readonly IEnumerable<RAnimal> Animals = [
            GD.Load<RAnimal>("res://expansion_packs/core-pet/animal/resources/Dog.tres"),
        ];

        public Main()
        {
            Actions.ForEach(ContextActionDB.Register);
            Animals.ForEach(AnimalDB.Register);
        }
    }
}