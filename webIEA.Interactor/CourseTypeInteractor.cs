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
    public class CourseTypeInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public CourseTypeInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }

        public object Add(CourseTypeDto model)
        {
            return repositoryWrapper.CourseTypeManager.Add(model);
        }
        public object Update(CourseTypeDto model)
        {
            return repositoryWrapper.CourseTypeManager.Update(model);
        }
        public List<CourseTypeDto> GetAll()
        {
            return repositoryWrapper.CourseTypeManager.GetAll();
        }
        public CourseTypeDto GetById(int id)
        {
            return repositoryWrapper.CourseTypeManager.GetById(id);
        }
        public object Delete(int id)
        {
            return repositoryWrapper.CourseMemberManager.Delete(id);
        }
    }
}
