using System;

namespace Hero.Shared.Repository
{
    public interface IRepository<T> : IReadOnlyRepository<T>
        where T : IAggregateRoot
    {
        void Save(T entity);
        void Add(T entity);
        void Remove(Guid id);
    }
}
