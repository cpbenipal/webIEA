
using System;
using System.Collections.Generic;
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Repositories;

namespace WebIEA.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WebIEAContext2 _webIEAContext2;
        private Dictionary<Type, object> _repos;
     
        public UnitOfWork(WebIEAContext2 webIEAContext2)
        {
            _webIEAContext2 = webIEAContext2;
        }


     

        #region Repository Manager
        public IRepositoryBase<T> GetRepository<T>() where T : class
        {
            if (_repos == null)
            {
                _repos = new Dictionary<Type, object>();
            }
            var type = typeof(T);
            if (!_repos.ContainsKey(type))
            {
                _repos[type] = new RepositoryBase<T>(_webIEAContext2);
            }
            return (IRepositoryBase<T>)_repos[type];
        }

        #endregion

       

    }
}
