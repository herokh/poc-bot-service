using System;
using System.Data.Common;

namespace Hero.Shared.DbContext
{
    public interface IDBContext : IDisposable
    {
        void CreateConnection();
        DbCommand CreateCommand();
        DbTransaction CreateTransaction();
    }
}
