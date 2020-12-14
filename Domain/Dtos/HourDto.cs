using System;

namespace Domain.Dtos
{
    public class HourDto : EntityDto
    {
        public TimeSpan Time { get; set; }
        public int DayOfWeek { get; set; }
        public bool IsEnabled { get; set; }
        public int CampId { get; set; }
    }
}
