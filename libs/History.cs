using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace UndoRedo
{
    public interface IItem
    {
        // Properties
        IEnumerable<Action> DoMethods { get; }
        IEnumerable<Action> UndoMethods { get; }
        float DoTime { get; set; }
        bool Renderable { get; }

        // Abstract methods
        string GetName();
        bool CanMerge(IItem other) => false;
        IEnumerable<IItem> Merge(IItem other) => [this, other];

        // Methods
        bool IsSame(IItem other) => GetType() == other.GetType() && GetName() == other.GetName();
        void Do()
        {
            DoTime = Time.GetTicksMsec();
            DoMethods
                .Select(method => Result.FromException(method, _ => $"Error performing method: {method.Method.Name}"))
                .WhereError()
                .ForEach(GD.PushError);
        }
        void Undo() => UndoMethods
                .Select(method => Result.FromException(method, _ => $"Error performing method: {method.Method.Name}"))
                .WhereError()
                .ForEach(GD.PushError);
    }

    public class History
    {
        public static class InputAction
        {
            public static readonly StringName Undo = "ui_undo";
            public static readonly StringName Redo = "ui_redo";
        }

        public Control? NotificationContainer = null;

        private readonly List<IItem> Stack = [];
        private int CurrentIndex = -1;

        // TODO UI

        public event Action<IItem>? BeforeDo;
        public event Action<IItem>? BeforeUndo;
        public event Action<IItem>? BeforeRedo;
        public event Action<IItem>? AfterDo;
        public event Action<IItem>? AfterUndo;
        public event Action<IItem>? AfterRedo;
        public event Action? Changed;

        public History()
        {
            AfterRedo += item => Render(item, false);
            AfterUndo += item => Render(item, true);
        }

        public I Add<I>(I item) where I : IItem
        {
            // Clear undo
            if (CurrentIndex > 0 && CurrentIndex < Stack.Count)
                Stack.RemoveRange(CurrentIndex, Stack.Count - CurrentIndex);

            // Add item to stack
            if (Stack.Pop() is IItem prevItem)
                Stack.AddRange(prevItem.Merge(item));
            else Stack.Add(item);

            CurrentIndex = Stack.Count - 1;

            // Perform item
            var currentItem = Stack[^1];
            BeforeDo?.Invoke(currentItem);
            currentItem.Do();
            AfterDo?.Invoke(currentItem);
            Changed?.Invoke();
            return (I)currentItem;
        }

        public void Undo()
        {
            if (Stack.Get(CurrentIndex) is IItem item)
            {
                BeforeUndo?.Invoke(item);
                item.Undo();
                CurrentIndex -= 1;
                AfterUndo?.Invoke(item);
                Changed?.Invoke();
            }
        }

        public void Redo()
        {
            if (Stack.Get(CurrentIndex + 1) is IItem item)
            {
                BeforeRedo?.Invoke(item);
                item.Do();
                CurrentIndex += 1;
                AfterRedo?.Invoke(item);
                Changed?.Invoke();
            }
        }

        public void Clear()
        {
            Stack.Clear();
            Changed?.Invoke();
        }

        private void Render(IItem item, bool isUndo = false)
        {
            if (NotificationContainer is null || !item.Renderable) return;
            var text = (isUndo ? "Undo " : "Redo ") + item.GetName();
            var label = new Label() { Text = text };
            label.OverrideStyleBox(new StyleBoxFlat() { BgColor = Colors.Black });
            NotificationContainer.AddChild(label);
            NotificationContainer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.BottomRight);
            NotificationContainer.GetTree().CreateTimer(3.0).Timeout += label.QueueFree;
        }
    }
}