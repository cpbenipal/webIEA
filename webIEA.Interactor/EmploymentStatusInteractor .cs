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
    public class EmploymentStatusInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public EmploymentStatusInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }


        public object Add(EmploymentStatusDto model)
        {
            return repositoryWrapper.EmploymentStatusManager.Add(model);
        }
        public object Update(EmploymentStatusDto model)
        {
            return repositoryWrapper.EmploymentStatusManager.Update(model);
        }
        public List<EmploymentStatusDto> GetAll()
        {
            return repositoryWrapper.EmploymentStatusManager.GetAll();
        }
        public EmploymentStatusDto GetById(int id)
        {
            return repositoryWrapper.EmploymentStatusManager.GetById(id);
        }
        public object Delete(int id)
        {
            return repositoryWrapper.CourseMemberManager.Delete(id);
        }
    }
}
