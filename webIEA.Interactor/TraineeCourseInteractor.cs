//using AutoMapper;
using Flexpage.Business.DTO;
using Flexpage.Domain.Abstract;
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
    public class TraineeCourseInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly IUnitOfWork _unitOfWork;

        protected readonly ILanguageRepository _ocessor;

        public TraineeCourseInteractor(IRepositoryWrapper _repositoryWrapper, ILanguageRepository ocessor, IUnitOfWork unitOfWork)
        {
            repositoryWrapper = _repositoryWrapper;
            _ocessor = ocessor;
            _unitOfWork = unitOfWork;
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
            return repositoryWrapper.TraineeCourseManager.Add(data);
        }
        public object AddUpdate(TraineeCourseDto model)
        {
            int res = 0;
            var courlangrepo = _unitOfWork.GetRepository<CourseLanguage>();
            if (model.Id == 0)
            {
                res = Add(model);

            }
            else
            {
                var result = Update(model);
                res = (int)model.Id;
                courlangrepo.DeleteList(x => x.CourseID == model.Id);
                courlangrepo.Save();


            }
            if (model.LanguageId != null)
            {
                foreach (var lng in model.LanguageId)
                {
                    var dt = new CourseLanguage
                    {
                        CourseID = res,
                        LanguageID = lng,
                        AddedOn = DateTime.Now
                    };
                    courlangrepo.Insert(dt);
                }
                courlangrepo.Save();
            }
            return res;
        }
        public object Update(TraineeCourseDto traineeCourseDto)
        {
            var data = repositoryWrapper.TraineeCourseManager.GetById(traineeCourseDto.Id);
            data.TrainingName = traineeCourseDto.TrainingName;
            data.Description = traineeCourseDto.Description;
            data.ValidatedHours = traineeCourseDto.ValidatedHours;
            data.IsShow = traineeCourseDto.IsShow;
            data.TypeID = traineeCourseDto.TypeID;
            data.StartDateTime = traineeCourseDto.StartDateTime;
            data.EndDateTime = traineeCourseDto.EndDateTime;
            data.Cost = (decimal)traineeCourseDto.Cost;
            data.IsFullTime = traineeCourseDto.IsFullTime;
            //data.Location = traineeCourseDto.Location;
            data.IsApproved = traineeCourseDto.IsApproved;
            data.StatusID = 1;
            return repositoryWrapper.TraineeCourseManager.Update(data);
        }
        public List<TraineeCourseDto> GetAll()
        {
            var model = repositoryWrapper.TraineeCourseManager.GetAll();
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
                StatusID = x.StatusID,
                IsApproved = x.IsApproved,
            }).ToList();
            return data;
        }
        public TraineeCourseDto GetData(int? TId)
        {
            var model = new TraineeCourseDto();
            if (TId != 0 && TId != null)
            {
                model = GetById((int)TId);
                model.LanguageId = _unitOfWork.GetRepository<CourseLanguage>().GetAllFiltered(x => x.CourseID == TId).Select(x => x.LanguageID).ToList();
            }
            var languages = _ocessor.GetLanguages();
            model.Languages = languages.Select(x => new ListCollectionDto() { Id = x.ID, Value = x.Name }).ToList();
            var coursetype = repositoryWrapper.CourseTypeManager.GetAll();
            model.CourseType = coursetype.Select(x => new ListCollectionDto() { Id = (int)x.ID, Value = x.Name }).ToList();
            return model;
        }
        public TraineeCourseDto GetById(int id)
        {
            TraineeCourseDto traineeCourseDto = new TraineeCourseDto();
            var model = repositoryWrapper.TraineeCourseManager.GetById(id); 
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
            traineeCourseDto.IsFullTime = model.IsFullTime;
            return traineeCourseDto;
        }
        public object Delete(int id)
        {
            return repositoryWrapper.TraineeCourseManager.Delete(id);
        }
        public object VerifyCourse(int id,int status)
        {
            var repo=_unitOfWork.GetRepository<TrainingCours>();
            var data = repo.GetById(id);
            data.StatusID=status;
            var dt=repo.Update(data);
            repo.Save();
            return dt;
        }
    }
}
