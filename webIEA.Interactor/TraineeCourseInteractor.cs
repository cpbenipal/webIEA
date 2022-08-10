//using AutoMapper;
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

        public object Add(TraineeCourseDto model)
        {
            return repositoryWrapper.TraineeCourseManager.Add(model);
        }
        public object AddUpdate(TraineeCourseDto model)
        {
            int res = 0;
            var courlangrepo = _unitOfWork.GetRepository<CourseLanguage>();
            if (model.Id == 0)
            {
                res = repositoryWrapper.TraineeCourseManager.Add(model);

            }
            else
            {
                var result = repositoryWrapper.TraineeCourseManager.Update(model);
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
        public object Update(TraineeCourseDto model)
        {
            return repositoryWrapper.TraineeCourseManager.Update(model);
        }
        public List<TraineeCourseDto> GetAll()
        {
            return repositoryWrapper.TraineeCourseManager.GetAll();
        }
        public TraineeCourseDto GetData(int? TId)
        {
            var model = new TraineeCourseDto();
            if (TId != 0 && TId != null)
            {
                model = repositoryWrapper.TraineeCourseManager.GetById((int)TId);
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
            return repositoryWrapper.TraineeCourseManager.GetById(id);
        }
        public object Delete(int id)
        {
            return repositoryWrapper.TraineeCourseManager.Delete(id);
        }
    }
}
