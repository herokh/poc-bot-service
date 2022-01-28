using Hero.Shared.DbContext;
using System;
using System.Data;

namespace Hero.Shared.UnitOfWork
{
    public class UnitOfWorkAdo : IUnitOfWork
    {

        private IDbTransaction _dbTransaction;
        private readonly IDBContext _dbContext;

        public UnitOfWorkAdo(IDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Commit()
        {
            if (_dbTransaction == null)
            {
                throw new InvalidOperationException("The db transaction is null.");
            }

            using (_dbTransaction)
            {
                _dbTransaction.Commit();
            }
            _dbTransaction = null;
        }

        public void Create()
        {
            _dbTransaction = _dbContext.CreateTransaction();
        }

        public IUnitOfWork CreateConnection()
        {
            _dbContext.CreateConnection();
            return this;
        }

        public IDbTransaction GetTransaction()
        {
            return _dbTransaction;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _dbTransaction != null)
            {
                _dbTransaction.Dispose();
                _dbTransaction = null;
                _dbContext.Dispose();
            }
        }

        public void Rollback()
        {
            if (_dbTransaction == null)
            {
                throw new InvalidOperationException("The db transaction is null.");
            }

            using (_dbTransaction)
            {
                _dbTransaction.Rollback();
            }
            _dbTransaction = null;
        }
    }
}
