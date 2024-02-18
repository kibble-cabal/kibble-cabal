using Godot;

public sealed partial class DateTimeSubSystemBase : Node
{
    [Signal]
    public delegate void TickedEventHandler();

    public static RDateTime? DateTime => SaveSubSystem.Current?.DateTime;
    public static int Time => DateTime?.Time ?? 0;

    private Timer Timer = new();

    public override void _EnterTree()
    {
        Timer.WaitTime = DT.TimeSpeed;
        Timer.Autostart = true;
        Timer.Connect(Timer.SignalName.Timeout, Callable.From(OnTimeout));
        AddChild(Timer);
    }

    private void OnTimeout()
    {
        if (DateTime is RDateTime dt)
        {
            dt.Time += 1;
            EmitSignal(SignalName.Ticked);
        }
    }
}

public sealed partial class DateTimeSubSystem : Singleton<DateTimeSubSystemBase>
{
    public static RDateTime? DateTime => DateTimeSubSystemBase.DateTime;
    public static int Time => DateTimeSubSystemBase.Time;
}