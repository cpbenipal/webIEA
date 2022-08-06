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


        public object Add(MemberSpecializationDto model)
        {
            return repositoryWrapper.MemberSpecializationManager.Add(model);
        }
        public object Update(MemberSpecializationDto model)
        {
            return repositoryWrapper.MemberSpecializationManager.Update(model);
        }
        public List<MemberSpecializationDto> GetAll()
        {
            return repositoryWrapper.MemberSpecializationManager.GetAll();
        }
        public List<MemberSpecializationDto> GetAllFiltered(long Id)
        {
            return repositoryWrapper.MemberSpecializationManager.GetAllFiltered(Id);
        }
     
        public MemberSpecializationDto GetById(int id)
        {
            return repositoryWrapper.MemberSpecializationManager.GetById(id);
        }
        public object Delete(int id)
        {
            return repositoryWrapper.MemberSpecializationManager.Delete(id);
        }
        //public object DeleteList(List<MemberSpecialization> list)
        //{
        //    return repositoryWrapper.MemberSpecializationManager.DeleteList(list);
        //}
    }
}
