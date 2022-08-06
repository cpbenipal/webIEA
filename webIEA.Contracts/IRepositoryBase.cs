using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace webIEA.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllFiltered(Expression<Func<T, bool>> expression);
        T GetById(object id);
        T FirstOrDefaultAsync(Expression<Func<T, bool>> expression);
        T Insert(T obj);
        List<T> InsertList(List<T> obj);
        T Update(T obj);
        object Delete(object id);
        void Save();
        object DeleteList(Expression<Func<T, bool>> expression);
    } 
}
