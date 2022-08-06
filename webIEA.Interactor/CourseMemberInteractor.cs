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
    public class CourseMemberInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public CourseMemberInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }

        public object Add(CourseMemberDto courseMemberDto)
        {
            return repositoryWrapper.CourseMemberManager.Add(courseMemberDto);
        }
        public object Update(CourseMemberDto courseMemberDto)
        {
            return repositoryWrapper.CourseMemberManager.Update(courseMemberDto);
        }
        public List<CourseMemberDto> GetAll()
        {
            return repositoryWrapper.CourseMemberManager.GetAll();
        }
        public List<CourseMemberDto> GetAllFiltered(long Id)
        {
            return repositoryWrapper.CourseMemberManager.GetAllFiltered(Id);
        }
        public CourseMemberDto GetById(int id)
        {
            return repositoryWrapper.CourseMemberManager.GetById(id);
        }
        public object Delete(int id)
        {
            return repositoryWrapper.CourseMemberManager.Delete(id);
        }
        //public object DeleteList(List<MemberTranieeCommission> list)
        //{
        //    return repositoryWrapper.CourseMemberManager.DeleteList(list);
        //}
    }
}
