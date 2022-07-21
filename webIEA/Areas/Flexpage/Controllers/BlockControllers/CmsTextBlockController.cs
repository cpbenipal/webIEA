using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;

namespace Flexpage.Controllers
{
    public class CmsTextBlockController : Controller
    {
        public readonly ICmsTextBlockProvider _cmsTextBlockProvider;
        public CmsTextBlockController(ICmsTextBlockProvider cmsTextBlockProvider)
        {
            _cmsTextBlockProvider = cmsTextBlockProvider;
        }

        public PartialViewResult CmsText(string alias)
        {
            var m = _cmsTextBlockProvider.Load(alias);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateCmsText(CmsTextEditorModel model, string command, string parameters)
        {
            try
            {
                ModelState.Clear();
                string c = command.ToLower().Trim();

                if (c == "changelanguage" && parameters is string)
                {
                    _cmsTextBlockProvider.SelectLanguage(model, parameters);
                }
                else if (c == "save")
                {
                    _cmsTextBlockProvider.Save(model);
                    return SettingsUpdatedSuccessfully();
                }
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/CmsText.cshtml", model);

            }
            catch
            {
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/CmsText.cshtml", model);
            }
        }

        public PartialViewResult SettingsUpdatedSuccessfully()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/BlockSettingsSaveSuccessfully.cshtml", "Block settings were successfully saved");
        }

    }
}