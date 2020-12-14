using System;
using System.Data;
using NHibernate;
using SharpArch.Domain.PersistenceSupport;

namespace Infraestructure.Repositories
{
    public class TransactionManager : ITransactionManager
    {
        private ISession session;
        private ITransaction transaction;

        public TransactionManager(ISession session)
        {
            this.session = session;
        }

        public IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            try
            {
                transaction = session.BeginTransaction(isolationLevel);
                return transaction;
            }
            catch
            {
                throw;
            }
        }

        public void CommitTransaction()
        {
            try
            {
                transaction.Commit();
            }
            catch
            {
                throw;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                throw;
            }
        }
    }
}
