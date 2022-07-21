using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Models;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using System;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class ContactsBrowserController : Controller
    {
        private readonly IFlexpageRepository _repository;
        private readonly IFlexpageSettings _settings;
        private readonly IFlexpage _flexpageProcessor;
        private readonly IContactProvider _contactProvider;

        public ContactsBrowserController(IFlexpageRepository repository, IFlexpageSettings settings, IFlexpage flexpageProcessor,
            IContactProvider contactProvider)
        {
            _flexpageProcessor = flexpageProcessor;
            _repository = repository;
            _settings = settings;
            _contactProvider = contactProvider;
        }

        public ActionResult BrowserContacts(string alias, int? id, int? shortcutID, string xml = "", eContactType type = eContactType.None)
        {
            try
            {
                BrowserContactsModel model = new BrowserContactsModel(_settings, _flexpageProcessor, Request.QueryString);
                if (!string.IsNullOrEmpty(xml))
                {
                    try
                    {
                        model.Load(_repository, alias, id, type, shortcutID, _contactProvider, xml, true);
                    }
                    catch
                    {
                        model.Load(_repository, alias, id, type, shortcutID, _contactProvider, null, true);
                    }
                }
                else
                {
                    model.Load(_repository, alias, id, type, shortcutID, _contactProvider, null, true);
                }
                model.IsStatic = true;

                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", model);

            }
            catch (Exception ex)
            {
#if DEBUG
                return PartialView("~/Areas/Flexpage/Views/Shared/Error.cshtml", String.Format("Sorry, there is something wrong with block '{0}'. Please contact your administrator. <br><b>Message:</b> {1} <br><b>Stacktrace:</b> {2}", alias, ex.Message, ex.StackTrace));
#else
                return PartialView("~/Areas/Flexpage/Views/Shared/Error.cshtml", String.Format("Sorry, there is something wrong with block '{0}'. Please contact your administrator.", alias));
#endif
            }
        }

    }
}