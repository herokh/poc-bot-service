using System.Data.Common;
using System.Data.SqlClient;

namespace Hero.Shared.Repository
{
    public class SqlClientProviderFactory : DbProviderFactory
    {
        public override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new SqlConnection();
        }

        public override DbParameter CreateParameter()
        {
            return new SqlParameter();
        }
    }
}
