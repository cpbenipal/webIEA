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

        public object Add(StatusDto model)
        {
            return repositoryWrapper.MemberStatusManager.Add(model);
        }
        public object Update(StatusDto model)
        {
            return repositoryWrapper.MemberStatusManager.Update(model);
        }
        public List<StatusDto> GetAll()
        {
            return repositoryWrapper.MemberStatusManager.GetAll();
        }
        public StatusDto GetById(int id)
        {
            return repositoryWrapper.MemberStatusManager.GetById(id);
        }
        public object Delete(int id)
        {
            return repositoryWrapper.MemberStatusManager.Delete(id);
        }
    }
}
