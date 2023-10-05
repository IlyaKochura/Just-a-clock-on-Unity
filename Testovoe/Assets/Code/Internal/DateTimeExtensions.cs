using System;

namespace Code.Internal
{
    public static class DateTimeExtensions
    {
        public static TimeSpan ToTimeSpan(this DateTime dateTime)
        {
            var timeSpan = new TimeSpan(0 , dateTime.Hour, dateTime.Minute, dateTime.Second);
            return timeSpan;
        }
    }
}