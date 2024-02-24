using System;
using System.Collections.Generic;
using UndoRedo;
using Godot;

namespace KibbleCabal.Core.Building.UndoRedo
{
    public struct Move(RBuilding building, int[] walls, int[] floors, int[] roofs, Vector2 delta) : IItem
    {
        public const string Name = "Move";
        public RBuilding Building = building;
        public int[] Walls = walls;
        public int[] Floors = floors;
        public int[] Roofs = roofs;
        public readonly IEnumerable<Action> DoMethods => [_Do];
        public readonly IEnumerable<Action> UndoMethods => [_Undo];
        public float DoTime { get; set; }
        public readonly bool Renderable => true;
        public readonly string GetName() => Name;
        private readonly void _Do()
        {
            Building.MoveBy<Wall>(Walls, delta);
            Building.MoveBy<Floor>(Floors, delta);
            Building.MoveBy<Roof>(Roofs, delta);
        }
        private readonly void _Undo()
        {
            Building.MoveBy<Wall>(Walls, -delta);
            Building.MoveBy<Floor>(Floors, -delta);
            Building.MoveBy<Roof>(Roofs, -delta);
        }
    }
}