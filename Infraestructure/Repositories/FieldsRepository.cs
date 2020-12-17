using System;
using Domain;
using Domain.RepositoryInterfaces;
using NHibernate;
using SharpArch.Domain.PersistenceSupport;

namespace Infraestructure.Repositories
{
    public class FieldsRepository : Repository<Field>, IFieldsRepository
    {
        public FieldsRepository(ISession session, ITransactionManager transactionManager) : base(session, transactionManager)
        {
            
        }

        public Field FindAvailable(DateTime date, Hour hour)
        {
            try
            {
                var query = Session.CreateSQLQuery("CALL SP_AVAILABLE_FIELD(:date, :hourId, :campId);").AddEntity(typeof(Field));
                query.SetDateTime("date", date);
                query.SetInt32("hourId", hour.Id);
                query.SetInt32("campId", hour.Camp.Id);
                return query.UniqueResult<Field>();
            }
            catch
            {
                throw;
            }
        }
    }
}
