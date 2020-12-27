using System;
using System.Collections.Generic;
using SharpArch.Domain.PersistenceSupport;

namespace Domain.RepositoryInterfaces
{
    public interface IFieldsRepository : IRepository<Field>
    {
        Field FindAvailable(DateTime date, Hour hour);

        Field Exists(string name, int campId);

        IList<Field> List(int campId);
    }
}
