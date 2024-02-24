using System;
using System.Collections.Generic;
using UndoRedo;

namespace KibbleCabal.Apps.DragDrop.UndoRedo
{
    public struct RemovePoint(int index, Action<int> removePoint, Action<int> addPoint) : IItem
    {
        public string Name = $"Remove Point";
        readonly int Index = index;
        readonly Action<int> RemovePointMethod = removePoint;
        readonly Action<int> AddPointMethod = addPoint;
        public readonly IEnumerable<Action> DoMethods => [_Do];
        public readonly IEnumerable<Action> UndoMethods => [_Undo];
        public float DoTime { get; set; }
        public readonly bool Renderable => true;
        public readonly string GetName() => Name;
        private readonly void _Do() => RemovePointMethod(Index);
        private readonly void _Undo() => AddPointMethod(Index);
    }
}