using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using System;
using System.Linq;
using System.Web.Mvc;
using FlexPage2.Areas.Flexpage.Infrastructure;
using System.Text;
using System.IO;
using Pluritech.Settings.Abstract;

namespace Flexpage.Controllers
{
    public class NewsController : BlockControllerBase
    {
        private readonly INewsBlockProvider _newsBlockProvider;
        private readonly IViewConverter _viewConverter;

        public NewsController(INewsBlockProvider newsBlockProvider, IViewConverter viewConverter, IFlexpageSettings settings, ILocalization localization) 
            : base(settings, localization)
        {
            _newsBlockProvider = newsBlockProvider;
            _viewConverter = viewConverter;
        }

        // GET: Flexpage/News
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult News(string alias)
        {
            NewsViewModel viewModel = _newsBlockProvider.Load(alias, Request.QueryString);
            viewModel.IsStatic = true;
            return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", viewModel);
        }

        [HttpPost]
        [FlexpageAdmin]
        public PartialViewResult UpdateNews(NewsEditorModel model, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if (c == "save")
                {
                    model.Categories = TokenBoxExtension.GetSelectedValues<string>("TagNameBlock").ToList();
                    model = _newsBlockProvider.Save(model);
                }
                return SettingsUpdatedSuccessfully();
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        protected PartialViewResult GetEditorFor(NewsEditorModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/News.cshtml", model);
        }

        [HttpPost]
        public string UploadNews(int blockID, string direction)
        {
            NewsViewModel viewModel = _newsBlockProvider.Load(blockID, Request.QueryString);
            string result = String.Format("<div id='uploadedNews_{0}' class='forAnimate {1}'>", viewModel.ID, direction == "forward" ? "uploadedNews_forward" : "uploadedNews_back");
            ViewBag.IsAdmin = viewModel.AdminMode;
            ViewBag.ViewTemplate = viewModel.TemplateView;
            ViewBag.ShowAdminControls = viewModel.ShowAdminControls;
            foreach (EventViewModel record in viewModel.Records)
            {
                result += _viewConverter.RenderPartialViewToString(this, "~/Areas/Flexpage/Views/Flexpage/NewsRecordContainer.cshtml", record);
            }
            result += "</div>";
            return result;
        }

        /// <summary>
        /// Create file .ics  by news id  and return file name
        /// </summary>
        /// <param name="id">News id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Calendar(EventViewModel model)
        {
            
            //Create the calendar
            var iCalendar = _newsBlockProvider.CreateCalendar(model);

            return File(new MemoryStream(Encoding.UTF8.GetBytes(iCalendar)), "text/calendar");
        }

    }
}