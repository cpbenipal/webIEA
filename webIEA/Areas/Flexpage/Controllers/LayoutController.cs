using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Domain.Abstract;
using Flexpage.Models;
using Pluritech.Permissions.Abstract;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class LayoutController : Controller
    {
        private readonly IFlexpageRepository _repository;
        private readonly IFlexpageSettings _settings;
        private readonly IPermissionsService _permissionsService;
        private readonly IFlexpage _flexpageProcessor;

        public LayoutController(IFlexpage flexpageProcessor, IFlexpageRepository repository, IFlexpageSettings settings, IPermissionsService permService)
        {
            _flexpageProcessor = flexpageProcessor;
            _repository = repository;
            _settings = settings;
            _permissionsService = permService;
        }
        
        public bool IsAdmin()
        {
            return _settings.IsCmsAdmin();
        }

        public bool IsContactsAdmin()
        {
            return _permissionsService.IsContactsAdmin(User.Identity.Name);
        }
        
        public string GetLang()
        {
            return _settings.GetCurrentOrDefaultLangCode();
        }

        public ActionResult GenerateResources()
        {
            bool session = Session != null;
            var result = _flexpageProcessor.GenerateResources(IsAdmin(), session ? Session["BlockTypes"] as HashSet<eBlockType> : null);
            if (session)
            {
                Session["BlockTypes"] = null;
            }
            return PartialView("~/Areas/Flexpage/Views/Shared/Resources.cshtml", result);
        }

        public ActionResult Disclaimer()
        {
            var model = new DisclaimerModel(_settings, _flexpageProcessor);
            model.Load(_repository, null);

            if (!model.Enabled)
                return null;

            return PartialView("~/Areas/Flexpage/Views/Shared/DisclaimerBanner.cshtml", model);
        }
    }
}