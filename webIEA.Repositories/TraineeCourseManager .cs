
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

        public TraineeCourseManager(IRepositoryBase<TrainingCours> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public int Add(TrainingCours model)
        {           
            var result = _repositoryBase.Insert(model);
            _repositoryBase.Save();
            return result.ID;
        }
        public object Update(TrainingCours trainingCours)
        {
            
            var result = _repositoryBase.Update(trainingCours);

            _repositoryBase.Save();
            return result;
        }
        public TrainingCours GetById(long Id)
        {
            var model = _repositoryBase.GetById(Id);
            return model;
        }
        public List<TrainingCours> GetAll()
        {
            var model = _repositoryBase.GetAll().ToList();            
            return model;
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
