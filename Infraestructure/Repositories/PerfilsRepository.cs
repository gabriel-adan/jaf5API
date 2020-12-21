using Domain;
using Domain.RepositoryInterfaces;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Domain.PersistenceSupport;

namespace Infraestructure.Repositories
{
    public class PerfilsRepository : Repository<Perfil>, IPerfilsRepository
    {
        public PerfilsRepository(ISession session, ITransactionManager transactionManager) : base(session, transactionManager)
        {

        }

        public Perfil Exists(string email)
        {
            try
            {
                return Session.CreateCriteria<Perfil>().Add(Restrictions.Where<Perfil>(p => p.Email == email)).UniqueResult<Perfil>();
            }
            catch
            {
                throw;
            }
        }
    }
}
