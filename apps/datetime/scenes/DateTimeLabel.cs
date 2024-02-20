using Godot;
using System;

namespace KibbleCabal.Apps.DateTime
{
    public partial class DateTimeLabel : Label
    {
        private static RDateTime? DateTime => SaveSubSystem.Current?.DateTime;

        public override void _Ready()
        {
            base._Ready();
            SaveSubSystem.Instance.SaveChanged += OnSaveChanged;
            OnSaveChanged();
        }

        private void OnSaveChanged()
        {
            if (DateTime is not null)
                DateTime.Changed += OnDateTimeChanged;
            OnDateTimeChanged();
        }

        private void OnDateTimeChanged()
        {
            if (DateTime is null) return;
            var time = new DT.DateTime(DateTime.Time);
            var timeString = $"{time.HourString}:{time.MinuteString}{time.PeriodString}";
            var dateString = $"{time.SeasonString} {time.DateString}, Y{time.YearString}";
            Text = $"{time.DayShortString}, {dateString}, {timeString}";
        }
    }
}