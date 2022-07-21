using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using webIEA.Contracts;
using webIEA.DataBaseContext;

namespace webIEA.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private WebIEAContext _context;
        private DbSet<T> table;
        public RepositoryBase()
        {
            this._context = new WebIEAContext();
            table = _context.Set<T>();
        }
        public RepositoryBase(WebIEAContext _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }
        public T GetById(object id)
        {
            return table.Find(id);
        }
        public T Insert(T obj)
        {
            table.Add(obj);
            return obj;
        }
        public T Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
            return obj;
        }
        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
