using SharpArch.Domain.DomainModel;

namespace Domain
{
    public class Customer : Entity
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
    }
}
