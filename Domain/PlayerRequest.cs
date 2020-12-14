using SharpArch.Domain.DomainModel;

namespace Domain
{
    public class PlayerRequest : Entity
    {
        public virtual bool IsConfirmed { get; set; }

        public virtual Player Player { get; set; }
        public virtual Turn Turn { get; set; }
    }
}
