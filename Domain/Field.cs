using SharpArch.Domain.DomainModel;

namespace Domain
{
    public class Field : Entity
    {
        public virtual string Name { get; set; }
        public virtual bool IsEnabled { get; set; }

        public virtual Camp Camp { get; set; }
    }
}
