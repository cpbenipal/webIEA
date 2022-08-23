
using System;
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class CourseTypeManager : ICourseTypeManager
    {
        private readonly IRepositoryBase<CourseType> _repositoryBase;

        public CourseTypeManager(IRepositoryBase<CourseType> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public object Add(CourseTypeDto courseTypeDto)
        {
            var data = new CourseType
            {
                Name = courseTypeDto.Name,
            };
            var result = _repositoryBase.Insert(data);
            return result;
        }
        public object Update(CourseTypeDto courseTypeDto)
        {
            var data = _repositoryBase.GetById(courseTypeDto.ID);
            data.Name = courseTypeDto.Name;
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;
        }
        public CourseTypeDto GetById(long Id)
        {
            CourseTypeDto courseTypeDto = new CourseTypeDto();
            var model = _repositoryBase.GetById(Id);
            courseTypeDto.ID = model.ID;
            courseTypeDto.Name = model.Name;
            return courseTypeDto;
        }
        public List<CourseTypeDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new CourseTypeDto
            {
                ID = x.ID,
                Name = x.Name,
            }).ToList();
            return data;
        }
        public List<CourseTypeDto> GetAllFiltered(long Id)
        {
            var model = _repositoryBase.GetAllFiltered(x=>x.ID==Id);
            var data = model.Select(x => new CourseTypeDto
            {
                ID = x.ID,
                Name = x.Name,
            }).ToList();
            return data;
        }
        public object Delete(int Id)
        {
            _repositoryBase.Delete(Id);
            _repositoryBase.Save();
            return "";
        }
    }
}
