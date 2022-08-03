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
         
        //public List<MemberSpecializationDto> GetAllSpecialization() 
        //{
        //    return repositoryWrapper.MemberSpecialization.GetAllSpecialization();
        //}
        //public MemberSpecializationDto GetSpecializationById(int id) 
        //{
        //    return repositoryWrapper.MemberSpecialization.GetSpecializationById(id);
        //}       
    }
}
