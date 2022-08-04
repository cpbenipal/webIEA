//using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class TraineeCourseInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public TraineeCourseInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }

        public object Add(TraineeCourseDto model)
        {
            return repositoryWrapper.TraineeCourseManager.Add(model);
        }
        public object Update(TraineeCourseDto model)
        {
            return repositoryWrapper.TraineeCourseManager.Update(model);
        }
        public List<TraineeCourseDto> GetAll()
        {
            return repositoryWrapper.TraineeCourseManager.GetAll();
        }
        public TraineeCourseDto GetById(int id)
        {
            return repositoryWrapper.TraineeCourseManager.GetById(id);
        }
        public object Delete(int id)
        {
            return repositoryWrapper.TraineeCourseManager.Delete(id);
        }
    }
}
