using Domain;
using Domain.RepositoryInterfaces;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Domain.PersistenceSupport;
using System;
using System.Collections.Generic;

namespace Infraestructure.Repositories
{
    public class TurnsRepository : Repository<Turn>, ITurnsRepository
    {
        public TurnsRepository(ISession session, ITransactionManager transactionManager) : base(session, transactionManager)
        {

        }

        public IList<Turn> GetRequests(DateTime date, Hour hour, Field field)
        {
            try
            {
                return Session.CreateCriteria<Turn>()
                    .Add(Restrictions.Where<Turn>(t => t.Field == field && t.Date == date && t.Hour == hour && t.State == EState.REQUESTED))
                    .List<Turn>();
            }
            catch
            {
                throw;
            }
        }

        public IList<Turn> ListByBufferZone(double longitude, double latitude, float radius, DateTime dateTime)
        {
            try
            {
                var query = Session.CreateSQLQuery(string.Format("CALL SP_TURNS_IN_BUFFER_ZONE({0}, {1}, {2}, '{3}');", longitude, latitude, radius, dateTime.ToString("yyyy-MM-dd HH:mm:ss")));
                query.AddEntity(typeof(Turn));
                return query.List<Turn>();
            }
            catch
            {
                throw;
            }
        }
    }
}
