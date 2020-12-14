using System;
using SharpArch.Domain.DomainModel;

namespace Domain
{
    public class Hour : Entity
    {
        public virtual TimeSpan Time { get; set; }
        public virtual int DayOfWeek { get; set; }
        public virtual bool IsEnabled { get; set; }

        public virtual Camp Camp { get; set; }
    }
}
