using Microsoft.Extensions.Configuration;
using System;
using System.Data.Common;

namespace Hero.Shared.DbContext
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly string _connectionString;

        public DbConnectionFactory(DbProviderFactory dbProviderFactory,
            IConfiguration configuration)
        {
            _dbProviderFactory = dbProviderFactory;
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }


        public DbConnection Create()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("The connection string is empty.");

            var dbConnection = _dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = _connectionString;
            dbConnection.Open();
            return dbConnection;
        }
    }
}
