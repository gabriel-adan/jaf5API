using System;
using SharpArch.Domain.DomainModel;

namespace Domain
{
    public class Account : Entity
    {
        public virtual DateTime CreatedDate { get; set; }
        public virtual bool IsEnabled { get; set; }

        public virtual Camp Camp { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
