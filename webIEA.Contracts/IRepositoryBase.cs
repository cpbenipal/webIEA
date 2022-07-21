using System.Collections.Generic;

namespace webIEA.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(object id);
        T Insert(T obj);
        T Update(T obj);
        void Delete(object id);
        void Save();
    } 
}
