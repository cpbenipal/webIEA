using Flexpage.Controllers;
using Flexpage.Models;
using Pluritech.Notifications.Abstract;
using Pluritech.Notifications.Abstract.Enum;
using Pluritech.Notifications.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flexpage.Domain.Abstract;
using Pluritech.Shared.Abstract;
using Pluritech.Shared.Abstract.DTO;
using Pluritech.Shared.Abstract.DTO.Extensions;

namespace Flexpage.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly INotificationsService _notificationsService;
        private readonly IObjectQueryBuilder _queryBuilder;

        public NotificationsController(INotificationsService notificationService, [Ninject.Named("FoldersQueryBuilder")]IObjectQueryBuilder queryBuilder)
        {
            _notificationsService = notificationService;
            _queryBuilder = queryBuilder;
        }

        public ActionResult Index()
        {
            return Redirect("/Account/login");
        }


        public PartialViewResult GetFolderNotifications(BlockCommandModel model)
        {
            FolderNotificationsEditorModel editorModel = _notificationsService.LoadFolderNotificationSettings(model.ID);
            return GetEditorFor(editorModel);
        }

        public PartialViewResult GetUserNotificationSettings(BlockCommandModel model)
        {
            UserNotificationsEditorModel editorModel = _notificationsService.LoadUserNotificationSettings();
            return GetEditorFor(editorModel);
        }

        public PartialViewResult NotificationSettings()
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/NotificationSettings.cshtml");
        }

        public PartialViewResult FolderNotificationsGrid()
        {
            var model = _notificationsService.LoadFolderNotificationSettings();
            var filterBy = new List<FilterDesciption>
            {
                new FilterDesciption()
                {
                    Table = "Folder", Field = "ID", Value = model.Select(m => m.FolderID).ToList(),
                }
            };
            var query = _queryBuilder.NewQuery();
            query.FilterBy(filterBy);
            var folders = _queryBuilder.ObjectsList();
            model.ForEach(m =>
            {
                m.Path = folders.FirstOrDefault(f => f.ID == m.FolderID)?.GetPropertyValue("Folder.Name").ToString();
            });

            return PartialView("~/Areas/Flexpage/Views/Flexpage/FolderNotificationGrid.cshtml", model);
        }

        [HttpPost]
        public ActionResult UpdateFolderNotificationSettings(FolderNotificationsEditorModel model, string command, string parameters)
        {
            _notificationsService.SaveFolderNotificationSettings(model);
            return GetEditorFor(model);
        }

        public void DeleteFolderNotificationSettings(int FolderID)
        {
            _notificationsService.DeleteFolderNotificationSettings(FolderID);
        }

        [HttpPost]
        public ActionResult UpdateUserNotificationSettings(UserNotificationsEditorModel model, string command, string parameters)
        {
            _notificationsService.SaveUserNotificationSettings(model);
            return GetEditorFor(model);
        }

        protected PartialViewResult GetEditorFor(FolderNotificationsEditorModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/FolderNotification.cshtml", model);
        }

        protected PartialViewResult GetEditorFor(UserNotificationsEditorModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/NotificationSettings.cshtml", model);
        }

    }
}