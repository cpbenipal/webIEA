using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Models;

namespace Flexpage.Controllers
{
    public class SitemapTreeViewBlockController : Controller
    {
        private readonly ISitemapTreeViewBlockProvider _sitemapBlockProvider;

        public SitemapTreeViewBlockController(ISitemapTreeViewBlockProvider sitemapBlockProvider)
        {
            _sitemapBlockProvider = sitemapBlockProvider;
        }

        public ActionResult Index()
        {
            return View();
        }


        //Partial view after sucessfully update settings of the block
        public PartialViewResult SettingsUpdatedSuccessfully()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/BlockSettingsSaveSuccessfully.cshtml", "Block settings were successfully saved");
        }

        public PartialViewResult SitemapTreeView(string alias)
        {
            // TODO: implement SitemapTreeView as a block
            //var model = _sitemapBlockProvider.Load(alias);

            //return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", model);

            var model = _sitemapBlockProvider.Load(alias);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/SitemapTreeView.cshtml", model);
        }

        public PartialViewResult GetSitemapTreeViewEditorContent(BlockCommandModel model)
        {
            throw new NotImplementedException();
        }

        protected PartialViewResult GetEditorFor(BreadcrumbsModel model)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateSitemapTreeView(BreadcrumbsModel model, string command, string parameters)
        {
            throw new NotImplementedException();

        }

    }
}