
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class MemberSpecializationManager : IMemberSpecialization
    {
        private readonly IRepositoryBase<MemberSpecialization> _repositoryBase; 
        private readonly Mapper mapper;

        public MemberSpecializationManager(IRepositoryBase<MemberSpecialization> repositoryBase)
        { 
            _repositoryBase = repositoryBase;          
        }

        //public object Add(MemberSpecializationDto memberSpecializationDto)
        //{
        //    var data = new MemberSpecialization
        //    {
        //        MemberID = memberSpecializationDto.MemberId,
        //        SpecializationId= memberSpecializationDto.sp
        //    };
        //    var result = _repositoryBase.Insert(data);
        //    return result;
        //}
        //public object Update(CourseTypeDto courseTypeDto)
        //{
        //    var data = _repositoryBase.GetById(courseTypeDto.ID);
        //    data.Name = courseTypeDto.Name;
        //    var result = _repositoryBase.Update(data);
        //    _repositoryBase.Save();
        //    return result;
        //}
        //public CourseTypeDto GetById(long Id)
        //{
        //    CourseTypeDto courseTypeDto = new CourseTypeDto();
        //    var model = _repositoryBase.GetById(Id);
        //    courseTypeDto.ID = model.ID;
        //    courseTypeDto.Name = model.Name;
        //    return courseTypeDto;
        //}
        //public List<CourseTypeDto> GetAll()
        //{
        //    var model = _repositoryBase.GetAll();
        //    var data = model.Select(x => new CourseTypeDto
        //    {
        //        ID = x.ID,
        //        Name = x.Name,
        //    }).ToList();
        //    return data;
        //}
        public object Delete(int Id)
        {
            var model = _repositoryBase.GetById(Id);
            _repositoryBase.Delete(model);
            _repositoryBase.Save();
            return "";
        }
    }
}
