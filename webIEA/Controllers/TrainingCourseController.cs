
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Controllers
{
    public class TrainingCourseController : Controller
    {
        // GET: IEAProviders/TrainingCourse
        private readonly TraineeCourseInteractor _traineeCourseInteractor;
        public TrainingCourseController(TraineeCourseInteractor traineeCourseInteractor)
        {
            _traineeCourseInteractor = traineeCourseInteractor;
        }
        public ActionResult Index()
        {
            List<TraineeCourseDto> model = new List<TraineeCourseDto>();

            try
            {
                model = _traineeCourseInteractor.GetAll();
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
        public ActionResult AddUpdate(int? id)
        {
            TraineeCourseDto model = new TraineeCourseDto();
            try
            {
                model = _traineeCourseInteractor.GetData(id);
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
        public ActionResult AddUpdateCourse(TraineeCourseDto traineeCourseDto)
        {
            //try
            //{
            if (ModelState.IsValid)
            {

                var result = _traineeCourseInteractor.AddUpdate(traineeCourseDto);
                return RedirectToAction("Index");
            }
            else
            {
                var model =  _traineeCourseInteractor.GetData((int?)traineeCourseDto.Id);
                return View("AddUpdate", model);
            }
        }
        public ActionResult VerifyCourse(int id, int status)
        {
            try
            {
                var result = _traineeCourseInteractor.VerifyCourse(id, status);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var result = _traineeCourseInteractor.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}