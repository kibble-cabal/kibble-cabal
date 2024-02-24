
using System;
using System.Collections.Generic;
using Godot;
using UndoRedo;

namespace KibbleCabal.Core.Building.UndoRedo
{
    public struct AddBuilding(RBuilding building, Vector2 position) : IItem
    {
        public const string Name = "Add Building";
        public RBuilding Building = building;
        public Vector2 Position = position;
        public readonly IEnumerable<Action> DoMethods => [_Do];
        public readonly IEnumerable<Action> UndoMethods => [_Undo];
        public float DoTime { get; set; }
        public readonly bool Renderable => true;
        public readonly string GetName() => Name;
        private void _Do()
        {
            Building.MoveBy(Position);
            Position = Vector2.Zero;
            LocationSubSystem.GetState()?.Add(new BuildingSpawner(Building));
        }
        private readonly void _Undo() => LocationSubSystem.GetState()?.RemoveSpawnersFor(Building);
    }
}