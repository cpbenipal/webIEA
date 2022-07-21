using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.Pluriworks.Service.Abstract;
using System.Web.Mvc;
using FlexPage2.Areas.Flexpage.Infrastructure;

namespace FlexPage.Controllers
{
    [FlexpageAdmin]
    public class LinkController : Controller
    {
        private readonly ILinkTypeManagerProvider _provider;

        public LinkController(ILinkTypeManagerProvider provider)
        {
            _provider = provider;
        }

        public ActionResult Index()
        {
            return LinkManager();
        }

        public ActionResult LinkManager()
        {
            return View("~/Areas/Flexpage/Views/Flexpage/LinkManager.cshtml");
        }

        public ActionResult LinkManagerGrid()
        {
            var model = _provider.GetLinkTypes();
            return PartialView("~/Areas/Flexpage/Views/Flexpage/LinkManagerGrid.cshtml", model);
        }

        public ActionResult LinkTypeAdd(LinkTypeModel model)
        {
            _provider.AddLinkType(model);
            return LinkManagerGrid();
        }
        public ActionResult LinkTypeEdit(LinkTypeModel model)
        {
            _provider.EditLinkType(model);
            return LinkManagerGrid();
        }
        public ActionResult LinkTypeDelete(int ID)
        {
            _provider.DeleteLinkType(ID);
            return LinkManagerGrid();
        }
    }
}