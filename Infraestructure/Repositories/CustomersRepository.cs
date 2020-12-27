using Domain;
using Domain.RepositoryInterfaces;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Domain.PersistenceSupport;

namespace Infraestructure.Repositories
{
    public class CustomersRepository : Repository<Customer>, ICustomersRepository
    {
        public CustomersRepository(ISession session, ITransactionManager transactionManager) : base(session, transactionManager)
        {

        }

        public Customer Exists(string email)
        {
            try
            {
                return Session.CreateCriteria<Customer>().Add(Restrictions.Where<Customer>(p => p.Email == email)).UniqueResult<Customer>();
            }
            catch
            {
                throw;
            }
        }
    }
}
