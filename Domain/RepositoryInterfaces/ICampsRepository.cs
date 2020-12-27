using SharpArch.Domain.PersistenceSupport;
using System.Collections.Generic;

namespace Domain.RepositoryInterfaces
{
    public interface ICampsRepository : IRepository<Camp>
    {
        IList<Camp> NearAround(double longitude, double latitude, decimal radius);
    }
}
