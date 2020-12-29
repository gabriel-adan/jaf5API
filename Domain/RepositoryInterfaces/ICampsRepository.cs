using SharpArch.Domain.PersistenceSupport;
using System.Collections.Generic;

namespace Domain.RepositoryInterfaces
{
    public interface ICampsRepository : IRepository<Camp>
    {
        IList<Camp> ListByBufferZone(double longitude, double latitude, float radius);
    }
}
