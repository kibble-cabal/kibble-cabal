
using System;
using System.Collections.Generic;
using Godot;
using UndoRedo;

namespace KibbleCabal.Apps.DragDrop.UndoRedo
{
    // TODO merge
    public struct MovePoint(Action<Vector3> setPosition, Vector3 start, Vector3 end) : IItem
    {
        public string Name = $"Move Point";
        readonly Vector3 EndPosition = end;
        readonly Vector3 StartPosition = start;
        readonly Action<Vector3> SetPosition = setPosition;
        public readonly IEnumerable<Action> DoMethods => [_Do];
        public readonly IEnumerable<Action> UndoMethods => [_Undo];
        public float DoTime { get; set; }
        public readonly bool Renderable => true;
        public readonly string GetName() => Name;
        private readonly void _Do() => SetPosition(StartPosition);
        private readonly void _Undo() => SetPosition(EndPosition);
    }
}