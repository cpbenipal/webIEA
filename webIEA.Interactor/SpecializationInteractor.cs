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
    public class SpecializationInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public SpecializationInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }
         
        public List<SpecializationDto> GetAllStatus() 
        {
            return repositoryWrapper.MemberSpecialization.GetAllSpecialization();
        }
        public SpecializationDto GetStatusById(int id) 
        {
            return repositoryWrapper.MemberSpecialization.GetSpecializationById(id);
        }       
    }
}
