using SharpArch.Domain.PersistenceSupport;

namespace Domain.RepositoryInterfaces
{
    public interface ICustomersRepository : IRepository<Customer>
    {
        Customer Exists(string email);
    }
}
