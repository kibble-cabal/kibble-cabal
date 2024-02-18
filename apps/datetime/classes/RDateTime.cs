using Godot;

public partial class RDateTime : ExtensibleResource
{
    private int _time = 0;

    [Export]
    public int Time
    {
        get => _time;
        set => this.Set(ref _time, value);
    }

    public int Year => DT.GetYear(Time);
    public int Season => DT.GetSeason(Time);
    public int Week => DT.GetWeek(Time);
    public int Date => DT.GetDate(Time);
    public int Day => DT.GetDay(Time);
    public int Hour => DT.GetHour(Time);
    public int Minute => DT.GetMinute(Time);
}