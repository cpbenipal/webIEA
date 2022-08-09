using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using webIEA.Contracts;
using webIEA.DataBaseContext;

namespace webIEA.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private WebIEAContext2 _context;
        private DbSet<T> table;
        public RepositoryBase()
        {
            this._context = new WebIEAContext2();
            table = _context.Set<T>();
        }

        public RepositoryBase(WebIEAContext2 _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }
        public IEnumerable<T> GetAllFiltered(Expression<Func<T, bool>> expression)
        {
            return table.Where(expression).ToList();
        }
        public T FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return table.FirstOrDefault(expression);
        }
        public T GetById(object id)
        {
            return table.Find(id);
        }
        public T GetByInclude(Expression<Func<T, object>>[] includes, Expression<Func<T, bool>> expression)
        {

            foreach (var include in includes)
            {
                table = (DbSet<T>)table.Include(include);
            }

            return table.FirstOrDefault(expression);
        }
        public IEnumerable<T> GetAllInclude(Expression<Func<T, bool>>[] includes)
        {

            foreach (var include in includes)
            {
                table = (DbSet<T>)table.Include(include);
            }

            return table.ToList();
        }
        public T Insert(T obj)
        {
            table.Add(obj);
            return obj;
        }
        public List<T> InsertList(List<T> obj)
        {
            table.AddRange(obj);
            return obj;
        }
        public T Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
            return obj;
        }
        public object Delete(int id)
        {
            T existing = table.Find(id);
            var res = table.Remove(existing);
            return res;
        }
        public object DeleteList(Expression<Func<T, bool>> expression)
        {
            var list = table.Where(expression).ToList();
            var res = table.RemoveRange(list);
            return res;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
