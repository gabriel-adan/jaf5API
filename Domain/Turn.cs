using System;
using System.Collections.Generic;
using SharpArch.Domain.DomainModel;

namespace Domain
{
    public class Turn : Entity
    {
        public virtual DateTime Date { get; set; }
        public virtual string FullName { get; set; }

        public virtual Hour Hour { get; set; }
        public virtual Field Field { get; set; }
        public virtual EState State { get; set; }
        public virtual Team Team { get; set; }
        public virtual IList<PlayerRequest> PlayerRequests { get; set; }
    }
}
