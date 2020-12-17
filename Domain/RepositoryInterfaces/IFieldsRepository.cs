using System;
using SharpArch.Domain.PersistenceSupport;

namespace Domain.RepositoryInterfaces
{
    public interface IFieldsRepository : IRepository<Field>
    {
        Field FindAvailable(DateTime date, Hour hour);
    }
}
