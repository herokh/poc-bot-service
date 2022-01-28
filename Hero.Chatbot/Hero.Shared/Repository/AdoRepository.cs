using Hero.Shared.DbContext;
using Hero.Shared.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Hero.Shared.Repository
{
    public abstract class AdoRepository<T> : ReadOnlyAdoRepository<T>, IRepository<T>
        where T : IAggregateRoot
    {
        public AdoRepository(DbProviderFactory dbProviderFactory, IDBContext dbContext, IUnitOfWork unitOfWork)
            : base(dbProviderFactory, dbContext, unitOfWork)
        {
        }

        public virtual void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> FindAll()
        {
            throw new NotImplementedException();
        }

        public virtual T FindBy(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual void Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
