using System;
using System.Collections.Generic;
using UndoRedo;

namespace KibbleCabal.Core.Building.UndoRedo
{
    public struct Destroy<T>(RBuilding building, T component) : IItem where T : IBuildingComponent<T>
    {
        public string Name = $"Destroy {typeof(T).Name}";
        public RBuilding Building = building;
        public T Component = component;
        public readonly IEnumerable<Action> DoMethods => [_Do];
        public readonly IEnumerable<Action> UndoMethods => [_Undo];
        public float DoTime { get; set; }
        public readonly bool Renderable => true;
        public readonly string GetName() => Name;
        private readonly void _Do() => Building.Remove(Component);
        private readonly void _Undo() => Building.Add(Component);
    }
}