using SharpArch.Domain.PersistenceSupport;

namespace Domain.RepositoryInterfaces
{
    public interface IPerfilsRepository : IRepository<Perfil>
    {
        Perfil Exists(string email);
    }
}
