using System.Collections.Generic;
using NHibernate;
using SharpArch.Domain.PersistenceSupport;

namespace Infraestructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ISession Session { get; }
        public ITransactionManager TransactionManager { get; set; }

        public Repository(ISession session, ITransactionManager transactionManager)
        {
            Session = session;
            TransactionManager = transactionManager;
        }

        public void Delete(T entity)
        {
            try
            {
                Session.Delete(entity);
            }
            catch
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                Session.Delete(id);
            }
            catch
            {
                throw;
            }
        }

        public void Evict(T entity)
        {
            try
            {
                Session.Evict(entity);
            }
            catch
            {
                throw;
            }
        }

        public T Get(int id)
        {
            try
            {
                return Session.Get<T>(id);
            }
            catch
            {
                throw;
            }
        }

        public IList<T> GetAll()
        {
            try
            {
                return Session.CreateCriteria<T>().List<T>();
            }
            catch
            {
                throw;
            }
        }

        public T Save(T entity)
        {
            try
            {
                Session.Save(entity);
                return entity;
            }
            catch
            {
                throw;
            }
        }

        public T SaveOrUpdate(T entity)
        {
            try
            {
                Session.SaveOrUpdate(entity);
                return entity;
            }
            catch
            {
                throw;
            }
        }
    }
}
