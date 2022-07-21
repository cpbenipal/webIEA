using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Flexpage.Models;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;

using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.Mvc;

namespace Flexpage.Controllers
{
    [FlexPage2.Areas.Flexpage.Infrastructure.FlexpageAdmin]
    public class ScheduleController : Controller
    {
        private readonly IScheduleProvider _scheduleProvider;

        public ScheduleController(IScheduleProvider scheduleProvider)
        {
            _scheduleProvider = scheduleProvider;
        }

        protected PartialViewResult GetEditorForItem(ScheduleItem viewModel)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/ScheduleItem.cshtml", viewModel);
        }

        protected ViewResult GetManagerFor(ScheduleManagerModel model)
        {
            return View("~/Areas/Flexpage/Views/Admin/ScheduleManager.cshtml", model);
        }

        protected PartialViewResult GetManagerForBlock(ScheduleManagerModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/Schedule.cshtml", model);
        }

        protected PartialViewResult GetManagerGridFor(List<ScheduleItem> scheduleItems)
        {
            return PartialView("~/Areas/Flexpage/Views/Admin/ScheduleManagerGrid.cshtml", scheduleItems);
        }

        public ViewResult LoadScheduleManager(BlockCommandModel commandModel)
        {
            ScheduleManagerModel scheduleModel = _scheduleProvider.LoadScheduleManager(commandModel.ID, false);
            return GetManagerFor(scheduleModel);
        }

        public PartialViewResult LoadManagerForLinkBlock(BlockCommandModel commandModel)
        {
            ScheduleManagerModel scheduleModel = _scheduleProvider.LoadScheduleManager(commandModel.ID, false);
            return GetManagerForBlock(scheduleModel);
        }

        public PartialViewResult LoadScheduleItem(BlockCommandModel commandModel)
        {
            ScheduleItem model = model = _scheduleProvider.LoadSchedule(commandModel.ID);
            return GetEditorForItem(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditScheduleItem(ScheduleItem editModel)
        {
            Appointment pattern = DevExpress.XtraScheduler.Compatibility.StaticAppointmentFactory.CreateAppointment(AppointmentType.Pattern);
            RecurrenceInfo recurrenceInfo = AppointmentRecurrenceFormExtension.GetValue("ScheduleRecurrenceForm", pattern);

            if(recurrenceInfo == null)
            {
                throw new Exception("Schedule`s reccurence form is missed or has incorrect format.");
            }

            recurrenceInfo.Start = editModel.StartDate;
            editModel.RecurrenceForm = recurrenceInfo;
            _scheduleProvider.EditScheduleItem(editModel);
            return GetEditorForItem(editModel);
        }

        public ActionResult fp_Schedule_Grid_Callback()
        {
            ScheduleManagerModel scheduleModel = _scheduleProvider.LoadScheduleManager(0, false);
            return GetManagerGridFor(scheduleModel.ScheduleItems);
        }

        public ActionResult fp_Schedule_Grid_CustomAction(string buttonID, int? scheduleID)
        {
            if (buttonID == "btnDeleteSchedule" && scheduleID.HasValue)
            {
                try
                {
                    _scheduleProvider.DeleteScheduleItem((int)scheduleID);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return fp_Schedule_Grid_Callback();
        }

        public JsonResult IsScheduleLinked(int scheduleID)
        {
            bool isLinked = false;
            try
            {
                isLinked = _scheduleProvider.IsScheduleLinked(scheduleID);
                
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Error ocured.\",\"message\" : \"" + e.Message.Split('.').First() + "\"}}");
            }


            if (isLinked)
            {
                return Json("{\"success\": true, \"hasBlock\": true}");
            }
            return Json("{\"success\": true, \"hasBlock\": false}");
        }

        [ValidateInput(false), HttpPost]
        public JsonResult ChangeActivationState(string scheduleID)
        {
            _scheduleProvider.ChangeActivationState(Convert.ToInt32(scheduleID));
            return Json("{\"success\": true}");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LinkBlockToSchedule(ScheduleManagerModel changedModel, string command)
        {
            ModelState.Clear();
            ScheduleManagerModel model = _scheduleProvider.LoadScheduleManager(changedModel.LinkedBlockID, changedModel.DisplayAllSchedules);
            model.LinkedBlockID = changedModel.LinkedBlockID;
            model.DisplayAllSchedules = changedModel.DisplayAllSchedules;
            if (command == "refresh")
            {
                foreach (ScheduleItem item in changedModel.ScheduleItems.Where(i => i.IsLinked))
                {
                    ScheduleItem originItem = model.ScheduleItems.FirstOrDefault(i => i.ID == item.ID);
                    if (originItem != null)
                    {
                        originItem.Hide = item.Hide;
                        originItem.AffectTM = item.AffectTM;
                        originItem.IsLinked = item.IsLinked;
                    }
                }
            }
            else if (command == "save")
            {
                foreach (ScheduleItem item in changedModel.ScheduleItems)
                {
                    _scheduleProvider.LinkToBlock(changedModel.LinkedBlockID, item);
                }
            }

            return GetManagerForBlock(model);
        }
    }
}