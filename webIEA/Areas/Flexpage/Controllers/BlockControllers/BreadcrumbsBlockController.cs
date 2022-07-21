using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Models;
using FlexPage2.Areas.Flexpage.Infrastructure;
using BreadcrumbsModel = Flexpage.Abstract.DTO.BreadcrumbsModel;

namespace Flexpage.Controllers
{
    public class BreadcrumbsBlockController : Controller
    {
        private readonly IBreadcrumbsBlockProvider _breadcrumbsBlockProvider;

        public BreadcrumbsBlockController(IBreadcrumbsBlockProvider breadcrumbsBlockProvider)
        {
            _breadcrumbsBlockProvider = breadcrumbsBlockProvider;
        }

        public PartialViewResult SettingsUpdatedSuccessfully()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/BlockSettingsSaveSuccessfully.cshtml", "Block settings were successfully saved");
        }

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Breadcrumbs(string alias)
        {
            BreadcrumbsModel model = _breadcrumbsBlockProvider.Load(alias);
            model.IsStatic = true;

            return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", model);
        }


        public PartialViewResult GetBreadcrumbsEditorContent(BlockCommandModel model)
        {
            BreadcrumbsModel breadcrumbsModel = null;

            if (model.ID > 0)
                breadcrumbsModel = _breadcrumbsBlockProvider.LoadForEditor(model.ID);
            else
                breadcrumbsModel = _breadcrumbsBlockProvider.LoadForEditor(model.BlockAlias);

            return GetEditorFor(breadcrumbsModel);
        }

        protected PartialViewResult GetEditorFor(BreadcrumbsModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/Breadcrumbs.cshtml", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [FlexpageAdmin]
        public ActionResult UpdateBreadcrumbs(BreadcrumbsModel model, string command, string parameters)
        {
            ModelState.Clear();
            try
            {
                string c = command.ToLower().Trim();
                if (c == "save")
                {
                    model = _breadcrumbsBlockProvider.Save(model);
                }
                return SettingsUpdatedSuccessfully();
            }
            catch (Exception)
            {
                return GetEditorFor(model);
            }
        }

    }
}