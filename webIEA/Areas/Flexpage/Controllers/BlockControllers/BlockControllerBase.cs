using Flexpage.Abstract;
using Pluritech.Settings.Abstract;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class BlockControllerBase : LocalizedController
    {
        public BlockControllerBase(IFlexpageSettings settings, ILocalization localization) : base(settings, localization) { }

        protected PartialViewResult SettingsUpdatedSuccessfully()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/BlockSettingsSaveSuccessfully.cshtml", "Block settings were successfully saved");
        }
    }
}