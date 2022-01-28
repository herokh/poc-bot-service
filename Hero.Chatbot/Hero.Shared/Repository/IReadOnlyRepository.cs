using System;
using System.Collections.Generic;

namespace Hero.Shared.Repository
{
    public interface IReadOnlyRepository<T>
        where T : IAggregateRoot
    {
        T FindBy(Guid id);
        IEnumerable<T> FindAll();
    }
}
