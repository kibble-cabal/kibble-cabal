using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

using CanMergeSignature = System.Func<HistoryItem, HistoryItem, bool>;
using MergeSignature = System.Func<HistoryItem, HistoryItem, HistoryItem>;

public partial class HistoryItem(string name, IEnumerable<Action> doMethods, IEnumerable<Action> undoMethods) : RefCounted
{
    public object? Caller;
    public string Name = name;
    public IEnumerable<Action> DoMethods { get; protected set; } = doMethods;
    public IEnumerable<Action> UndoMethods { get; protected set; } = undoMethods;
    public float DoTime { get; protected set; } = Time.GetTicksMsec();
    public bool Renderable = true;

    public bool IsSame(HistoryItem other) => GetType() == other.GetType() && Name == other.Name;
    public void Do() => DoMethods.ForEach(method => method());
    public void Undo() => UndoMethods.ForEach(method => method());
    public virtual bool CanMerge(HistoryItem other) => false;
    public virtual IEnumerable<HistoryItem> Merge(HistoryItem other) => [this, other];
}

public partial class MergableHistoryItem(string name, IEnumerable<Action> doMethods, IEnumerable<Action> undoMethods) : HistoryItem(name, doMethods, undoMethods)
{
    public CanMergeSignature? CanMergeMethod;
    public MergeSignature? MergeMethod;

    public MergableHistoryItem(string name, IEnumerable<Action> doMethods, IEnumerable<Action> undoMethods, CanMergeSignature? canMergeMethod, MergeSignature? mergeMethod) : this(name, doMethods, undoMethods)
    {
        this.CanMergeMethod = canMergeMethod;
        this.MergeMethod = mergeMethod;
    }

    public override bool CanMerge(HistoryItem other)
    {
        if (!base.CanMerge(other)) return false;
        if (!IsSame(other) || (other.DoTime - DoTime).Abs() > 10_000) return false;
        if (CanMergeMethod is not null)
            return CanMergeMethod(this, other);
        return true;
    }

    public override IEnumerable<HistoryItem> Merge(HistoryItem other)
    {
        if (!CanMerge(other)) return [this, other];
        if (MergeMethod is not null) return [MergeMethod(this, other)];
        this.DoMethods = [.. this.DoMethods, .. other.DoMethods];
        this.UndoMethods = [.. this.UndoMethods, .. other.UndoMethods];
        this.DoTime = other.DoTime;
        return [this];
    }
}

[GlobalClass]
public partial class History : RefCounted
{
    public static class InputAction
    {
        public static readonly StringName Undo = "ui_undo";
        public static readonly StringName Redo = "ui_redo";
    }

    private Array<HistoryItem> Stack = [];
    private Array<HistoryItem> UndoneStack = [];

    // TODO UI

    [Signal]
    public delegate void BeforeDoEventHandler(HistoryItem item);

    [Signal]
    public delegate void BeforeUndoEventHandler(HistoryItem item);

    [Signal]
    public delegate void BeforeRedoEventHandler(HistoryItem item);

    [Signal]
    public delegate void AfterDoEventHandler(HistoryItem item);

    [Signal]
    public delegate void AfterUndoEventHandler(HistoryItem item);

    [Signal]
    public delegate void AfterRedoEventHandler(HistoryItem item);

    [Signal]
    public delegate void ChangedEventHandler();

    public HistoryItem Add(string name, IEnumerable<Action> doMethods, IEnumerable<Action> undoMethods) => Add(new HistoryItem(name, doMethods, undoMethods));
    public HistoryItem Add(string name, Action? doMethod, Action? undoMethod) => Add(name, doMethod is not null ? [doMethod] : [], undoMethod is not null ? [undoMethod] : []);
    public MergableHistoryItem MergeAdd(string name, IEnumerable<Action> doMethods, IEnumerable<Action> undoMethods, CanMergeSignature? canMergeMethod = null, MergeSignature? mergeMethod = null) => Add(new MergableHistoryItem(name, doMethods, undoMethods, canMergeMethod, mergeMethod));
    public MergableHistoryItem MergeAdd(string name, Action? doMethod, Action? undoMethod, CanMergeSignature? canMergeMethod = null, MergeSignature? mergeMethod = null) => MergeAdd(name, doMethod is not null ? [doMethod] : [], undoMethod is not null ? [undoMethod] : [], canMergeMethod, mergeMethod);

    public I Add<I>(I item) where I : HistoryItem
    {
        // Add item to stack
        if (Stack.Pop() is HistoryItem prevItem)
            Stack.AddRange(prevItem.Merge(item));
        else Stack.Add(item);

        // Clear undo
        UndoneStack.Clear();

        // Perform item
        var currentItem = Stack[^1];
        EmitSignal(SignalName.BeforeDo, [currentItem]);
        currentItem.Do();
        EmitSignal(SignalName.AfterDo, [currentItem]);
        EmitChanged();

        return (I)currentItem;
    }

    public void Undo()
    {
        if (Stack.Pop() is HistoryItem item)
        {
            EmitSignal(SignalName.BeforeUndo, [item]);
            item.Undo();
            UndoneStack.Add(item);
            EmitSignal(SignalName.AfterUndo, [item]);
            EmitChanged();
        }
    }

    public void Redo()
    {
        if (UndoneStack.Pop() is HistoryItem item)
        {
            EmitSignal(SignalName.BeforeRedo, [item]);
            item.Do();
            Stack.Add(item);
            EmitSignal(SignalName.AfterRedo, [item]);
            EmitChanged();
        }
    }

    public void Clear()
    {
        Stack.Clear();
        UndoneStack.Clear();
        EmitChanged();
    }

    public void OnBeforeDo(string itemName, Action<HistoryItem> method) => BeforeDo += item => IfMatches(itemName, method)(item);
    public void OnBeforeUndo(string itemName, Action<HistoryItem> method) => BeforeUndo += item => IfMatches(itemName, method)(item);
    public void OnBeforeRedo(string itemName, Action<HistoryItem> method) => BeforeRedo += item => IfMatches(itemName, method)(item);
    public void OnAfterDo(string itemName, Action<HistoryItem> method) => AfterDo += item => IfMatches(itemName, method)(item);
    public void OnAfterUndo(string itemName, Action<HistoryItem> method) => AfterUndo += item => IfMatches(itemName, method)(item);
    public void OnAfterRedo(string itemName, Action<HistoryItem> method) => AfterRedo += item => IfMatches(itemName, method)(item);

    private Action<HistoryItem> IfMatches(string itemName, Action<HistoryItem> method) => item =>
    {
        if (item.Name == itemName) method(item);
    };

    private void EmitChanged() => EmitSignal(SignalName.Changed);
}