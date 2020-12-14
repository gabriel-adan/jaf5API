using SharpArch.Domain.PersistenceSupport;
using System;
using System.Collections.Generic;

namespace Domain.RepositoryInterfaces
{
    public interface IHoursRepository : IRepository<Hour>
    {
        IList<Hour> ToList(int campId);

        bool Exists(TimeSpan time, int dayOfWeek, int campId);
    }
}
