
using System;
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class CourseMemberManager : ICourseMemberManager
    {
        private readonly IRepositoryBase<MemberTranieeCommission> _repositoryBase;

        public CourseMemberManager(IRepositoryBase<MemberTranieeCommission> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public object Add(CourseMemberDto courseMemberDto)
        {
            var data = new MemberTranieeCommission
            {
                MemberID = courseMemberDto.MemberID,
                TrainingCourseId = courseMemberDto.TrainingCourseId,
                AddedOn=DateTime.Now,
                ModifiedOn=DateTime.Now,
              
            };
            var result = _repositoryBase.Insert(data);
            _repositoryBase.Save();

            return result;
        }
        public object Update(CourseMemberDto courseMemberDto)
        {
            var data = _repositoryBase.GetById(courseMemberDto.Id);
            data.MemberID = courseMemberDto.MemberID;
            data.TrainingCourseId = courseMemberDto.TrainingCourseId;
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;
        }
        public CourseMemberDto GetById(long Id)
        {
            CourseMemberDto courseMemberDto = new CourseMemberDto();
            var model = _repositoryBase.GetById(Id);
            courseMemberDto.Id = model.Id;
            courseMemberDto.TrainingCourseId = model.TrainingCourseId;
            courseMemberDto.MemberID = model.MemberID;
            return courseMemberDto;
        }
        public List<CourseMemberDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new CourseMemberDto
            {
                Id = x.Id,
                MemberID = x.MemberID,
                TrainingCourseId = x.TrainingCourseId,
            }).ToList();
            return data;
        }
        public List<CourseMemberDto> GetAllFiltered(long Id)
        {
            var model = _repositoryBase.GetAllFiltered(x=>x.MemberID==Id);
            var data = model.Select(x => new CourseMemberDto
            {
                Id = x.Id,
                MemberID = x.MemberID,
                TrainingCourseId = x.TrainingCourseId,
            }).ToList();
            return data;
        }
        public object Delete(int Id)
        {;
            _repositoryBase.Delete(Id);
            _repositoryBase.Save();
            return "";
        }
        public object DeleteList(long Id)
        {
            _repositoryBase.DeleteList(x=>x.MemberID==Id);
            _repositoryBase.Save();
            return "";
        }
    }
}
