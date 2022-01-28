using System;
using System.Data;

namespace Hero.Shared.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUnitOfWork CreateConnection();
        IDbTransaction GetTransaction();
        void Create();
        void Commit();
        void Rollback();
    }
}
