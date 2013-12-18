using System;
using System.Collections.Generic;

namespace CsiCrm
{
    public interface IRepository<T> : IEnumerable<T>
    {
        void Load();
        void Save();

        void Add(T item);
        bool Remove(T item);
    }
}
