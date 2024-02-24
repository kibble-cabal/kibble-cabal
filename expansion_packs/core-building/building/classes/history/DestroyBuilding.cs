
using System;
using System.Collections.Generic;
using UndoRedo;

namespace KibbleCabal.Core.Building.UndoRedo
{
    public struct DestroyBuilding(RBuilding building) : IItem
    {
        public const string Name = "Destroy Building";
        public RBuilding Building = building;
        public readonly IEnumerable<Action> DoMethods => [_Do];
        public readonly IEnumerable<Action> UndoMethods => [_Undo];
        public float DoTime { get; set; }
        public readonly bool Renderable => true;
        public readonly string GetName() => Name;
        private readonly void _Do() => LocationSubSystem.GetState()?.RemoveSpawnersFor(Building);
        private readonly void _Undo() => LocationSubSystem.GetState()?.Add(new BuildingSpawner(Building));
    }
}