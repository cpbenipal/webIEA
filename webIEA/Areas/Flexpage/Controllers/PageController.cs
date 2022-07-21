using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using FlexPage2.Areas.Flexpage.Infrastructure;

namespace Flexpage.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageProvider _pageProvider;

        public PageController(IPageProvider pageProvider)
        {
            _pageProvider = pageProvider;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create new page
        /// </summary>
        /// <param name="alias">Alias of the page that need to be created</param>
        /// <returns></returns>

        [FlexpageAdmin]
        public RedirectResult CreatePage(string alias)
        {
            //create new page from alias
            _pageProvider.CreatePage(alias);
            return Redirect("/" + alias);
        }

        [FlexpageAdmin]
        public ViewResult PageSettings(string alias, string ajaxUpdateTargedID)
        {
            var pageModel = _pageProvider.LoadPage(alias);
            pageModel.ShowSaveButton = true;
            pageModel.ShowUrlEditor = false;
            pageModel.AjaxUpdateTargetID = ajaxUpdateTargedID;
            pageModel.ShowPreviewButton = false;

            return View("~/Areas/Flexpage/Views/Flexpage/Editors/Page.cshtml", pageModel);
        }

        [FlexpageAdmin]
        public ActionResult PagesManager()
        {
            return View("~/Areas/Flexpage/Views/Admin/PagesManager.cshtml");
        }
        [FlexpageAdmin]
        public ActionResult PagesGrid()
        {
            var pages = _pageProvider.LoadPages();           
            return PartialView("~/Areas/Flexpage/Views/Admin/PagesGrid.cshtml", pages);
        }

        [FlexpageAdmin]
        [HttpPost]
        public PartialViewResult UpdatePage(PageModel model, string command, string parameters)
        {
            model.RobotsMetatagParameters = model.RobotsMetatagParameters ?? "";            
            ModelState.Clear();
            try
            {
                string c = command.ToLower().Trim();

                if(c == "changelanguage" && parameters is string)
                {
                    _pageProvider.Update(model);
                    _pageProvider.SelectLocalization(model, parameters);
                    return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/PageGeneral.cshtml", model);
                }
                else if(c == "save")
                {
                    if(ModelState.IsValid)
                    {
                        _pageProvider.Update(model);
                        _pageProvider.SavePage(model);
                        ViewBag.command = command;
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/Page.cshtml", model);
        }

        /// <summary>
        /// Action to delete a page
        /// </summary>
        /// <param name="pageID">ID of a block</param>
        /// <returns></returns>
        [FlexpageAdmin]
        public ActionResult DeletePage([ModelBinder(typeof(DevExpressEditorsBinder))] int ID)
        {
            _pageProvider.DeletePage(ID);

            return PagesGrid();
        }

        public MvcHtmlString GenerateMetatags(string alias)
        {
            PageHeaders headers = GetPageHeaders(alias);
            return new MvcHtmlString(headers.MetaTags);
        }

        public MvcHtmlString GenerateCustomCss(string alias)
        {
            PageHeaders headers = GetPageHeaders(alias);
            return new MvcHtmlString(headers.CustomCss);
        }

        public MvcHtmlString GenerateCustomJs(string alias)
        {
            PageHeaders headers = GetPageHeaders(alias);
            return new MvcHtmlString(headers.CustomJs);
        }

        private PageHeaders GetPageHeaders(string alias)
        {
            PageHeaders headers = new PageHeaders();

            if(ControllerContext.RequestContext.RouteData.Values["alias"].ToString() == alias && _pageProvider.IsPageExists(alias))
            {
                PageModel pageModel = _pageProvider.LoadPage(alias);
                headers.MetaTags = _pageProvider.GenerateMetatagsHtml(pageModel);
                headers.CustomCss = string.Format("<style type=\"text/css\">{0}</style>", pageModel.CustomCSS);
                headers.CustomJs = string.Format("<script type=\"text/javascript\">{0}</script>", pageModel.CustomJS);
            }
            else
                headers.MetaTags = $"<title>{ViewBag.Title}</title><meta charset='utf-8' /><meta name='viewport' content='width=device-width, initial-scale=1.0'/><meta name='robots' content='index, follow'/>";
            return headers;
        }
    }
}