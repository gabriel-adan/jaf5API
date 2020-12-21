using SharpArch.Domain.DomainModel;

namespace Domain
{
    public class Perfil : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
    }
}
