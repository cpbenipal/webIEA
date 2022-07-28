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
    public class MemberStatusInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public MemberStatusInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }
         
        public List<StatusDto> GetAllStatus() 
        {
            return repositoryWrapper.MemberStatusManager.GetAllStatus();
        }
        public StatusDto GetStatusById(int id) 
        {
            return repositoryWrapper.MemberStatusManager.GetStatusById(id);
        }       
    }
}
