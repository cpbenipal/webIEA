using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Helpers;
using Pluritech.Settings.Abstract;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    /// <summary>
    /// This is a common ancestor of all controllers that support localization.
    /// </summary>
    public abstract class LocalizedController : MaintenanceController
    {
        private readonly LocalizationHelper _localizer;
        protected readonly ILocalization _localization;
        public LocalizedController(IFlexpageSettings settings, ILocalization localization)
            :base()
        {
            _localization = localization;
            _localizer = new LocalizationHelper(settings, _localization);
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            _localizer.SetCurrentThreadCulture();
            _localizer.SaveLanguageToCookies(requestContext.HttpContext.ApplicationInstance.Request, requestContext.HttpContext.ApplicationInstance.Response);
            base.Initialize(requestContext);
        }
    }
}