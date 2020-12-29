using SharpArch.Domain.PersistenceSupport;
using System;
using System.Collections.Generic;

namespace Domain.RepositoryInterfaces
{
    public interface ITurnsRepository : IRepository<Turn>
    {
        IList<Turn> GetRequests(DateTime date, Hour hour, Field field);

        IList<Turn> ListByBufferZone(double longitude, double latitude, float radius, DateTime dateTime);;
    }
}
