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
    public class MemberSpecializationInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public MemberSpecializationInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }
         
        //public List<SpecializationDto> GetAllSpecialization() 
        //{
        //    return repositoryWrapper.MemberSpecialization.GetAllSpecialization();
        //}
        //public SpecializationDto GetSpecializationById(int id) 
        //{
        //    return repositoryWrapper.MemberSpecialization.GetSpecializationById(id);
        //}       
    }
}
