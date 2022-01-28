using System;
using System.Data.Common;

namespace Hero.Shared.DbContext
{
    public class DBContext : IDBContext
    {
        private DbConnection _dbConnection;

        private readonly IDbConnectionFactory _dbConnctionFactory;

        public DBContext(IDbConnectionFactory dbConnctionFactory)
        {
            _dbConnctionFactory = dbConnctionFactory;
        }

        public DbCommand CreateCommand()
        {
            if (_dbConnection == null)
            {
                throw new InvalidOperationException("The db connection is null.");
            }

            var command = _dbConnection.CreateCommand();
            command.CommandTimeout = 30;
            return command;
        }

        public void CreateConnection()
        {
            if (_dbConnctionFactory == null)
            {
                throw new InvalidOperationException("The db connection factory is null.");
            }

            _dbConnection = _dbConnctionFactory.Create();
        }

        public DbTransaction CreateTransaction()
        {
            if (_dbConnection == null)
            {
                throw new InvalidOperationException("The db connection is null.");
            }

            return _dbConnection.BeginTransaction();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
                _dbConnection = null;
            }
        }
    }
}
