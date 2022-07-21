using Flexpage.Abstract;
using FlexPage2.Areas.Flexpage.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    [FlexpageAdmin]
    public class WebsitesController : Controller
    {
        private readonly IWebsiteProvider _provider;
        public WebsitesController(IWebsiteProvider provider)
        {
            _provider = provider;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WebsitesGrid()
        {
            return PartialView("~/Areas/Flexpage/Views/Websites/WebsitesGrid.cshtml", _provider.GetWebsites().ToList());
        }
    }
}