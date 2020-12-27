using System.Collections.Generic;
using Domain;
using Domain.RepositoryInterfaces;
using NHibernate;
using SharpArch.Domain.PersistenceSupport;

namespace Infraestructure.Repositories
{
    public class CampsRepository : Repository<Camp>, ICampsRepository
    {
        public CampsRepository(ISession session, ITransactionManager transactionManager) : base(session, transactionManager)
        {

        }

        public override Camp Get(int id)
        {
            try
            {
                return Session.CreateSQLQuery(string.Format("SELECT Id, Name, Street, Number, IsEnabled, ST_X(Location) AS Longitude, ST_Y(Location) AS Latitude FROM camp WHERE Id = {0};", id)).AddEntity(typeof(Camp)).UniqueResult<Camp>();
            }
            catch
            {
                throw;
            }
        }

        public override Camp Save(Camp entity)
        {
            try
            {
                var query = Session.CreateSQLQuery(string.Format("INSERT INTO Camp (Name, Street, Number, IsEnabled, Location) VALUES ('{0}', '{1}', '{2}', {3}, ST_GeomFromText('POINT({4} {5})', 4326));", entity.Name, entity.Street, entity.Number, entity.IsEnabled, entity.Longitude, entity.Latitude));
                query.UniqueResult();
                var selectQuery = Session.CreateSQLQuery("SELECT Id, Name, Street, Number, IsEnabled, ST_X(Location) AS Longitude, ST_Y(Location) AS Latitude FROM Camp WHERE Id = LAST_INSERT_ID();");
                selectQuery.AddEntity(typeof(Camp));
                entity = selectQuery.UniqueResult<Camp>();
                return entity;
            }
            catch
            {
                throw;
            }
        }

        public override Camp SaveOrUpdate(Camp entity)
        {
            try
            {
                if (entity.IsTransient())
                {
                    return Save(entity);
                }
                else
                {
                    var query = Session.CreateSQLQuery(string.Format("UPDATE Camp SET Name = '{0}', Street = '{1}', Number = '{2}', IsEnabled = {3}, Location = ST_GeomFromText('POINT({4} {5})', 4326) WHERE Id = {6};", entity.Name, entity.Street, entity.Number, entity.IsEnabled, entity.Longitude, entity.Latitude, entity.Id));
                    query.UniqueResult();
                    return entity;
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<Camp> NearAround(double longitude, double latitude, decimal radius)
        {
            try
            {
                var query = Session.CreateSQLQuery(string.Format("SELECT Id, Name, Street, Number, IsEnabled, ST_X(Location) AS Longitude, ST_Y(Location) AS Latitude FROM camp WHERE ST_Intersects(ST_Buffer(ST_GeomFromText('POINT({0} {1})', 4326), {2}), Location) = 1;", longitude, latitude, radius));
                query.AddEntity(typeof(Camp));
                return query.List<Camp>();
            }
            catch
            {
                throw;
            }
        }
    }
}
