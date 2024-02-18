using System.Diagnostics;
using Godot;

public static class DT
{
    /// <summary>
    /// The amount of real-world seconds equal to one in-game minute.
    /// </summary>
    public const float TimeSpeed = 5.0f;
    public const int SeasonsInYear = 4;
    public const int WeeksInSeason = 4;
    public const int DaysInWeek = 7;
    public const int HoursInDay = 24;
    public const int MinutesInHour = 60;
    public const int MinutesInDay = MinutesInHour * HoursInDay;
    public const int MinutesInWeek = MinutesInDay * DaysInWeek;
    public const int MinutesInSeason = MinutesInWeek * WeeksInSeason;
    public const int MinutesInYear = MinutesInSeason * SeasonsInYear;

    public enum Season
    {
        Spring = 1,
        Summer = 2,
        Fall = 3,
        Winter = 4
    }

    public enum Day
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }

    public static StringName GetSeasonName(Season season) => season switch
    {
        Season.Spring => "Spring",
        Season.Summer => "Summer",
        Season.Fall => "Fall",
        Season.Winter => "Winter",
        _ => throw new UnreachableException()
    };

    public static StringName GetDayName(Day day) => day switch
    {
        Day.Monday => "Monday",
        Day.Tuesday => "Tuesday",
        Day.Wednesday => "Wednesday",
        Day.Thursday => "Thursday",
        Day.Friday => "Friday",
        Day.Saturday => "Saturday",
        Day.Sunday => "Sunday",
        _ => throw new UnreachableException()
    };

    public static StringName GetShortDayName(Day day) => ((string)GetDayName(day)).Substr(0, 3);

    private static int Ceil(int a, int b) => Mathf.CeilToInt(a / b);

    public static int GetYear(int time) => Ceil(time, MinutesInYear);
    public static int GetSeason(int time) => Ceil(time % MinutesInYear, MinutesInSeason);
    public static int GetWeek(int time) => Ceil(time % MinutesInSeason, MinutesInWeek);
    public static int GetWeekOfYear(int time) => Ceil(time % MinutesInYear, MinutesInWeek);
    public static int GetDay(int time) => Ceil(time % MinutesInWeek, MinutesInDay);
    public static int GetDate(int time) => Ceil(time % MinutesInSeason, MinutesInDay);
    public static int GetHour(int time) => Ceil(time % MinutesInDay, MinutesInHour);
    public static int GetMinute(int time) => time % MinutesInHour;

    public struct DateTime
    {
        public int Time;
        public readonly int Year => DT.GetYear(Time);
        public readonly int Season => DT.GetSeason(Time);
        public readonly int Week => DT.GetWeek(Time);
        public readonly int Date => DT.GetDate(Time);
        public readonly int Day => DT.GetDay(Time);
        public readonly int Hour => DT.GetHour(Time);
        public readonly int Minute => DT.GetMinute(Time);
    }
}
