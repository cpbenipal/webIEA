using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Models;
using FlexPage2.Areas.Flexpage.Infrastructure;

namespace Flexpage.Controllers
{
    public class MenuBlockController : Controller
    {
        private readonly ISitemapMenuBlockProvider _sitemapMenuBlockProvider;

        public MenuBlockController(ISitemapMenuBlockProvider sitemapMenuBlockProvider)
        {
            _sitemapMenuBlockProvider = sitemapMenuBlockProvider;
        }

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult SettingsUpdatedSuccessfully()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/BlockSettingsSaveSuccessfully.cshtml", "Block settings were successfully saved");
        }

        public PartialViewResult SitemapMenu(string alias,bool isStatic = true)
        {
            SitemapMenuModel menuModel = _sitemapMenuBlockProvider.Load(alias);
            menuModel.IsStatic = isStatic;

            return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", menuModel);
        }


        public PartialViewResult GetSitemapMenuEditorContent(BlockCommandModel model)
        {
            SitemapMenuEditModel menuModel = null;

            if (model.ID > 0)
                menuModel = _sitemapMenuBlockProvider.LoadForEditor(model.ID);
            else
                menuModel = _sitemapMenuBlockProvider.LoadForEditor(model.BlockAlias);

            return GetEditorFor(menuModel);
        }

        protected PartialViewResult GetEditorFor(SitemapMenuEditModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/SitemapMenu.cshtml", model);
        }

        public PartialViewResult GetSitemapMenuContent(BlockCommandModel model)
        {
            SitemapMenuModel menuModel = null;

            if (model.ID > 0)
                menuModel = _sitemapMenuBlockProvider.Load(model.ID);
            else
                menuModel = _sitemapMenuBlockProvider.Load(model.BlockAlias);

            return null;
        }

        [HttpPost]
        [ValidateInput(false)]
        [FlexpageAdmin]
        public ActionResult UpdateSitemapMenu(SitemapMenuEditModel model, string command, string parameters)
        {
            ModelState.Clear();
            try
            {
                string c = command.ToLower().Trim();
                if (c == "changesitemap" && parameters is string)
                {
                    _sitemapMenuBlockProvider.FillModelData(model);
                }
                else if (c == "save")
                {
                    model = _sitemapMenuBlockProvider.Save(model);
                    return SettingsUpdatedSuccessfully();
                }
                return GetEditorFor(model);
            }
            catch (Exception)
            {
                return GetEditorFor(model);
            }
        }

    }
}