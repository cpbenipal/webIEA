using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Models;
using System.Web.Mvc;
using Pluritech.UserProfile.Abstract;

namespace Flexpage.Controllers
{
    public class NavigationBarController : Controller
    {
        protected readonly IFlexpageSettings _settings;
        protected readonly IPageProvider _pageProvider;
        protected readonly IUserGeneralPermissionProvider _permissionProvider;
        public NavigationBarController(IPageProvider pageProvider, IFlexpageSettings settings, IUserGeneralPermissionProvider permissionProvider)
        {
            _settings = settings;
            _pageProvider = pageProvider;
            _permissionProvider = permissionProvider;
        }

        public PartialViewResult NavigationBar(string alias)
        {
            PageModel page = _pageProvider.LoadPage(alias);
            var model = new NavigationBarModel()
            {
                AdminMode = _settings.IsCmsAdmin(),
                UserName = User.Identity.Name,
                PageName = page != null ? alias : "",
                BlockID = page != null ? page.ID : -1,
                ShowAdminControls = _settings.ShowAdminControls(),
                IsPWAdmin = _permissionProvider.IsPWAdmin()
            };
            return PartialView("~/Areas/Flexpage/Views/Flexpage/NavigationBar.cshtml", model);
        }
    }
}