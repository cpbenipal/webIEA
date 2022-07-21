using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DevExpress.Web.Mvc;
using Flexpage.Abstract;

namespace Flexpage.Controllers
{
    [FlexPage2.Areas.Flexpage.Infrastructure.FlexpageAdmin]
    public class EventsController : Controller
    {
        private readonly IEventsProvider _eventsProvider;

        public EventsController(IEventsProvider eventsProvider)
        {
            _eventsProvider = eventsProvider;
        }

        // GET: Flexpage/Events
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EventsGrid()
        {
            var model = _eventsProvider.LoadEventsGrid();
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/EventsGrid.cshtml", model);
        }

        /// <summary>
        /// Action to delete the news record
        /// </summary>
        /// <param name="ID">ID of the record</param>
        /// <returns></returns>
        public ActionResult DeleteEvent([ModelBinder(typeof(DevExpressEditorsBinder))] int ID)
        {
            try
            {
                _eventsProvider.DeleteEvent(ID);
            }
            catch {
                return EventsGrid();
            }
            return EventsGrid();
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateEvent(Flexpage.Abstract.DTO.EventEditorModel model, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if (!model.CategoryID.HasValue)
                    model.CategoryName = ComboBoxExtension.GetValue<string>("CategoryID");

                if (c == "changelanguage" && parameters is string)
                {
                    ModelState.Clear();
                    _eventsProvider.SelectLanguage(model, parameters);
                }
                else if (c == "save")
                {
                    model = _eventsProvider.Save(model);
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateEventCategory(Flexpage.Abstract.DTO.EventCategoryEditorModel model, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if (model.LanguageSelector == null)
                {
                }

                if (c == "changelanguage" && parameters is string)
                {
                    ModelState.Clear();
                    _eventsProvider.ChangeCategoryLanguage(model, parameters);
                }
                else if (c == "save")
                {
                    model = _eventsProvider.Save(model);
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        protected PartialViewResult GetEditorFor(Flexpage.Abstract.DTO.EventEditorModel model)
        {
            switch (model.BlockType)
            {
                case "CalendarEvent":
                    return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/CalendarEvent.cshtml", model);
                default:
                    return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/Event.cshtml", model);
            }
        }

        protected PartialViewResult GetEditorFor(Flexpage.Abstract.DTO.EventCategoryEditorModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/EventCategory.cshtml", model);
        }

    }
}