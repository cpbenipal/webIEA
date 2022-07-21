using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Flexpage.Models;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;

namespace Flexpage.Controllers
{
    [FlexPage2.Areas.Flexpage.Infrastructure.FlexpageAdmin]
    public class SitemapManagerController : Controller
    {
        private readonly ISitemapProvider _sitemapProvider;

        public SitemapManagerController(ISitemapProvider sitemapProvider)
        {
            _sitemapProvider = sitemapProvider;
        }

        protected PartialViewResult GetEditorFor(SitemapManagerModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/SitemapManagerDB.cshtml", model);
        }

        public PartialViewResult GetSitemapManagerContent(BlockCommandModel model)
        {
            var sitemapModel = _sitemapProvider.Load();

            return GetEditorFor(sitemapModel);
        }
        public PartialViewResult GetEditorForMigrate(string model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/SiteMapManagerDBMigrate.cshtml", model);
        }
        [HttpPost]
        public PartialViewResult UpdateSitemapMigrate(string LoadedFileName, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if (c == "migratesitemap")
                {
                    _sitemapProvider.MigrateSitemapFile(LoadedFileName);
                    ViewBag.Result = "Migration successful";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMassage= ex.Message;
            }
            return GetEditorForMigrate(LoadedFileName);
        }
        [HttpPost]
        public PartialViewResult UpdateSitemap(SitemapManagerModel model, string command, string parameters)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string c = command.ToLower().Trim();
                    if (c == "save")
                    {
                        _sitemapProvider.Save(model);
                        model = _sitemapProvider.Load();
                    }
                    else if (c == "changesitemap")
                    {
                        _sitemapProvider.LoadSitemap(model, parameters);
                    }
                    else if (c == "addsitemap")
                    {
                        _sitemapProvider.AddSitemap(model, parameters);
                    }
                    else if (c == "addnewnode")
                    {
                        _sitemapProvider.AddNewNode(model);
                    }
                    else if (c == "canceledit")
                    {
                        _sitemapProvider.CancelEdit(model, Convert.ToInt32(parameters));
                    }
                    else if (c == "deletenode")
                    {
                        _sitemapProvider.DeleteNode(model, Convert.ToInt32(parameters));
                    }
                    else
                    {
                        _sitemapProvider.InitSitemapData(model);
                    }
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return GetEditorFor(model);
        }

    }
}