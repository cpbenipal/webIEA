using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FlexPage2.Areas.Flexpage.Infrastructure;
using Pluritech.Authentication.Abstract;

namespace Flexpage.Controllers
{
    public class CalendarBlockController : Controller
    {
        private readonly ICalendarBlockProvider _calendarBlockProvider;
        private readonly IEventsProvider _eventsProvider;
        private readonly IFlexpageSettings _settings;
        private readonly IUser _user;
        public CalendarBlockController(ICalendarBlockProvider calendarBlockProvider, IEventsProvider eventsProvider, IFlexpageSettings settings, IUser user)
        {
            _calendarBlockProvider = calendarBlockProvider;
            _eventsProvider = eventsProvider;
            _settings = settings;
            _user = user;
        }

        public ActionResult SmallCalendar(int modelId, string date,string langCode)
        {
            CalendarViewModel calendarModel = _calendarBlockProvider.Load(modelId);
            calendarModel.LanguageSelector.CurrentLangCode = langCode;
            return PartialView("~/Areas/Flexpage/Views/Flexpage/CalendarSmall.cshtml", calendarModel);
        }

        public PartialViewResult SettingsUpdatedSuccessfully()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/BlockSettingsSaveSuccessfully.cshtml", "Block settings were successfully saved");
        }

        [HttpPost]
        [ValidateInput(false)]
        [FlexpageAdmin]
        public ActionResult UpdateCalendar(CalendarViewModel calendarModel, string command, string parameters)
        {
            ModelState.Clear();

            string c = command.ToLower().Trim();
            try
            {
                calendarModel.Categories = TokenBoxExtension.GetSelectedValues<string>("TagNameBlock").ToList();
                switch (c)
                {
                    case "save":
                        calendarModel = _calendarBlockProvider.Save(calendarModel);
                        return SettingsUpdatedSuccessfully();
                    case "changelanguage":
                        calendarModel = _calendarBlockProvider.ChangeLanguage(calendarModel, parameters);
                        break;
                }
                return GetEditorFor(calendarModel);
            }
            catch
            {
                return GetEditorFor(calendarModel);
            }
        }

        public ActionResult Categories(eCalendarActionTypeEnum command, int id = -1)
        {
            List<EventCategoryInfoModel> model;
            switch (command)
            {
                case eCalendarActionTypeEnum.CategoryGrid:
                     model = _eventsProvider.LoadCategories();
                    return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/EventCategoriesGrid.cshtml", model);
                case eCalendarActionTypeEnum.EditCategory:
                case eCalendarActionTypeEnum.AddCategory:
                    var category = id > 0 ? _eventsProvider.LoadCategoryForEditor(id) : _eventsProvider.CreateCategoryForEditor();
                    return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/EventCategoryPopup.cshtml", category);
                case eCalendarActionTypeEnum.DeleteCategory:
                    try
                    {
                        _eventsProvider.DeleteCategory(id);
                    }
                    catch(Exception ex) {
                        ViewData["DeleteError"] = ex.Message;
                    }
                    model = _eventsProvider.LoadCategories();
                    return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/EventCategoriesGrid.cshtml", model);
                default:
                    return null;
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangeLanguageCategory(EventCategoryEditorModel model, string lang)
        {
            _eventsProvider.ChangeCategoryLanguage(model, lang);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/EventCategory.cshtml", model);
        }

        [FlexpageAdmin]
        public ActionResult AddEditCategory(EventCategoryEditorModel model)
        {
            _eventsProvider.Save(model);
            return null;
        }

        protected PartialViewResult GetEditorFor(CalendarViewModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/Calendar.cshtml", model);
        }

        public ActionResult AddEventToFavorites(DateTime? date, int calendarID, int eventID, string url)
        {
            _calendarBlockProvider.AddEventToFavorites(eventID, _user.ID, url);
            return CalendarEventPopup(eCalendarActionTypeEnum.ShowEvents, date, calendarID);
        }
        
        public ActionResult CalendarEventPopup(eCalendarActionTypeEnum command, DateTime? date, int id)
        {
            var records = _calendarBlockProvider.LoadEventsForDay(date, id);
            var calendar = _calendarBlockProvider.Load(id);
            ViewBag.CurrentDay = date;
            ViewBag.CalendarBlockID = id;
            ViewBag.IsAdmin = _settings.IsCurrentPageAdmin();
            ViewBag.AllowFavorites = calendar.AllowFavorites;
            if (command == eCalendarActionTypeEnum.ShowPopupEvents)
            {
                return View("~/Areas/Flexpage/Views/Flexpage/Editors/Calendar/CalendarEventsPopup.cshtml", records);
            }

            if (command == eCalendarActionTypeEnum.ShowEvents)
            {
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/Calendar/CalendarEventsListPopup.cshtml", records);
            }

            return null;
        }
    }
}