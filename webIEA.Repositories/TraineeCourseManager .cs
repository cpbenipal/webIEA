
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
    public class TraineeCourseManager : ITraineeCourseManager
    {
        private readonly IRepositoryBase<TrainingCours> _repositoryBase;
        private readonly Mapper mapper;

        public TraineeCourseManager(IRepositoryBase<TrainingCours> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public int Add(TraineeCourseDto traineeCourseDto)
        {
            var data = new TrainingCours
            {
                TrainingName = traineeCourseDto.TrainingName,
                Description = traineeCourseDto.Description,
                ValidatedHours = traineeCourseDto.ValidatedHours,
                IsShow = traineeCourseDto.IsShow,
                TypeID = traineeCourseDto.TypeID,
                StartDateTime = traineeCourseDto.StartDateTime,
                EndDateTime = traineeCourseDto.EndDateTime,
                Cost = (decimal)traineeCourseDto.Cost,
                IsFullTime = traineeCourseDto.IsFullTime,
                Location = traineeCourseDto.Location,
                IsApproved = traineeCourseDto.IsApproved,
                StatusID = 1,
                AddedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };
            var result = _repositoryBase.Insert(data);
            _repositoryBase.Save();
            return result.ID;
        }
        public object Update(TraineeCourseDto traineeCourseDto)
        {
            var data = _repositoryBase.GetById(traineeCourseDto.Id);
            data.TrainingName = traineeCourseDto.TrainingName;
            data.Description = traineeCourseDto.Description;
            data.ValidatedHours = traineeCourseDto.ValidatedHours;
            data.IsShow = traineeCourseDto.IsShow;
            data.TypeID = traineeCourseDto.TypeID;
            data.StartDateTime = traineeCourseDto.StartDateTime;
            data.EndDateTime = traineeCourseDto.EndDateTime;
            data.Cost = (decimal)traineeCourseDto.Cost;
            data.IsFullTime = traineeCourseDto.IsFullTime;
            data.Location = traineeCourseDto.Location;
            data.IsApproved = traineeCourseDto.IsApproved;
            data.StatusID = traineeCourseDto.StatusID;
            var result = _repositoryBase.Update(data);

            _repositoryBase.Save();
            return result;
        }
        public TraineeCourseDto GetById(long Id)
        {
            TraineeCourseDto traineeCourseDto = new TraineeCourseDto();
            var model = _repositoryBase.GetById(Id);
            traineeCourseDto.Id = model.ID;
            traineeCourseDto.TrainingName = model.TrainingName;
            traineeCourseDto.Description = model.Description;
            traineeCourseDto.ValidatedHours = (int)model.ValidatedHours;
            traineeCourseDto.IsShow = model.IsShow;
            traineeCourseDto.TypeID = (int)model.TypeID;
            traineeCourseDto.StartDateTime = (DateTime)model.StartDateTime;
            traineeCourseDto.EndDateTime = (DateTime)model.EndDateTime;
            traineeCourseDto.Location = model.Location;
            traineeCourseDto.IsApproved = model.IsApproved;
            traineeCourseDto.StatusID = model.StatusID;
            return traineeCourseDto;
        }
        public List<TraineeCourseDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new TraineeCourseDto
            {
                Id = x.ID,
                TrainingName = x.TrainingName,
                Description = x.Description,
                ValidatedHours = (int)x.ValidatedHours,
                IsShow = x.IsShow,
                TypeID = x.TypeID,
                StartDateTime = x.StartDateTime,
                EndDateTime = x.EndDateTime,
                Cost = (float)x.Cost,
                IsFullTime = x.IsFullTime,
                Location = x.Location,
                IsApproved = x.IsApproved,
            }).ToList();
            return data;
        }
        //public List<TraineeCourseDto> GetAllIncluded()
        //{
        //    var model = _repositoryBase.GetAllInclude(x=>x.CourseType);
        //    var data = model.Select(x => new TraineeCourseDto
        //    {
        //        Id = x.ID,
        //        TrainingName = x.TrainingName,
        //        Description = x.Description,
        //        ValidatedHours = (int)x.ValidatedHours,
        //        IsShow = x.IsShow,
        //        TypeID = x.TypeID,
        //        StartDateTime = x.StartDateTime,
        //        EndDateTime = x.EndDateTime,
        //        Cost = (float)x.Cost,
        //        IsFullTime = x.IsFullTime,
        //        Location = x.Location,
        //        IsApproved = x.IsApproved,
        //    }).ToList();
        //    return data;
        //}
        public object Delete(int Id)
        {
            _repositoryBase.Delete(Id);
            _repositoryBase.Save();
            return "";
        }
    }
}
