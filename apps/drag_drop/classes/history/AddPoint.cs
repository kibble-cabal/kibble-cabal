using System;
using System.Collections.Generic;
using UndoRedo;

namespace KibbleCabal.Apps.DragDrop.UndoRedo
{
    public struct AddPoint(int index, Action<int> addPoint, Action<int> removePoint) : IItem
    {
        public string Name = $"Add Point";
        readonly int Index = index;
        readonly Action<int> AddPointMethod = addPoint;
        readonly Action<int> RemovePointMethod = removePoint;
        public readonly IEnumerable<Action> DoMethods => [_Do];
        public readonly IEnumerable<Action> UndoMethods => [_Undo];
        public float DoTime { get; set; }
        public readonly bool Renderable => true;
        public readonly string GetName() => Name;
        private readonly void _Do() => AddPointMethod(Index);
        private readonly void _Undo() => RemovePointMethod(Index);
    }
}