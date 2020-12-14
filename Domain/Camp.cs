using System;
using SharpArch.Domain.DomainModel;
using System.Collections.Generic;

namespace Domain
{
    public class Camp : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Street { get; set; }
        public virtual string Number { get; set; }
        public virtual bool IsEnabled { get; set; }

        public virtual IList<Hour> Hours { get; set; }
        public virtual IList<Field> Fields { get; set; }
    }
}
