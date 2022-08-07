//using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class UnitOfWorkInteractor<T> where T : class
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public UnitOfWorkInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }

        //public object Add(T model)
        //{
        //    return repositoryWrapper.UnitOfWork.GetRepository<T>().Insert(model);
        //}
        //public object Update(T model)
        //{
        //    return repositoryWrapper.UnitOfWork.GetRepository<T>().Update(model);

        //}
        public List<T> GetAll()
        {
            return (List<T>)repositoryWrapper.UnitOfWorkManager.GetRepository<T>();

        }
        //public IEnumerable<T> GetAllFiltered(Expression<Func<T, bool>> expression)
        //{
        //    var repo = repositoryWrapper.UnitOfWork.GetRepository<T>();
        //    return repo.GetAllFiltered(expression);

        //}
        //public T GetById(int id)
        //{
        //    return repositoryWrapper.UnitOfWork.GetRepository<T>().GetById(id);

        //}
        //public object Delete(int id)
        //{
        //    return repositoryWrapper.UnitOfWork.GetRepository<T>().Delete(id);

        //}
        //public object DeleteList(Expression<Func<T, bool>> expression)
        //{
        //    return repositoryWrapper.UnitOfWork.GetRepository<T>().DeleteList(expression);

        //}
    }
}
