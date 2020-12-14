using System;

namespace WebApi.Models
{
    public class HourForm
    {
        public int Id { get; set; }
        public TimeSpan Time { get; set; }
        public int DayOfWeek { get; set; }
        public bool IsEnabled { get; set; }
        public int CampId { get; set; }
    }
}
