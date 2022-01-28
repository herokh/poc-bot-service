using System.Data.Common;

namespace Hero.Shared.DbContext
{
    public interface IDbConnectionFactory
    {
        DbConnection Create();
    }
}
