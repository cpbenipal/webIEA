using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Abstract.Enum;
using Pluritech.Properties.Abstract;
using Pluritech.Properties.Abstract.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class ParticipantsBlockController : Controller
    {
        private readonly IParticipantsBlockProvider _provider;
        private readonly IPropertyProvider _propertyService;
        public ParticipantsBlockController(IParticipantsBlockProvider provider, IPropertyProvider propertyService)
        {
            _provider = provider;
            _propertyService = propertyService;
        }
        
        public ActionResult Index(int id)
        {
            return View();
        }

        public JsonResult GetParicipantImageProperty(int contactType)
        {
            var properties = new List<Property>();
            if ((int)ParticipantsContactType.Persons == contactType || (int)ParticipantsContactType.Both == contactType)
            {
                properties.AddRange(_propertyService.EnumerateProperties(ObjectType.PersonObject).Where(x => x.Type == ObjectPropertyType.File));
            }
            if((int)ParticipantsContactType.Companies == contactType || (int)ParticipantsContactType.Both == contactType)
            {
                foreach (var property in _propertyService.EnumerateProperties(ObjectType.CompanyObject).Where(x => x.Type == ObjectPropertyType.File))
                {
                    if(!properties.Any(p => p.ID == property.ID))
                    {
                        properties.Add(property);
                    }
                }
            }
            var result = properties.Select(x => new { Caption = x.Caption, Name = x.Name });
            return Json(result);
        }

        public JsonResult GetParticipants(int blockID, int itemsCount, int page, int countryId, string firstName, string lastName, string organization)
        {
            List<ParticipantInfoViewModel> cache = page != 0 ? Session["ParticipantsCache" + blockID] as List<ParticipantInfoViewModel> : null;
            if (cache == null || cache.Count == 0)
            {
                Session["ParticipantsCache" + blockID] = cache = _provider.GetParticipants(blockID, countryId, firstName, lastName, organization);
            }
            List<ParticipantInfoViewModel> participants = itemsCount > 0 ? cache.Skip(page * itemsCount).Take(itemsCount).ToList() : cache;
            return Json(new { Count = cache.Count, Participants = participants });
        }

        public PartialViewResult Editor(ParticipantsOptionsModel model)
        {
            model.ContactsPerPage = 4;
            return PartialView(model);
        }

        public ActionResult Update(ParticipantsOptionsModel model)
        {
            _provider.Update(model);
            return Json("{\"success\": true, \"title\" : \"Paraticipants Block\",\"message\" : \"Participants Block updated successfully\"}");
        }

        public JsonResult GetParticipantsExtraData(int blockID, List<int> ids)
        {
            List<ParticipantInfoViewModel> cache = Session["ParticipantsCache" + blockID] as List<ParticipantInfoViewModel>;
            if (cache == null)
            {
                throw new System.Exception("Participants cache is empty");
            }

            var participantsViewModel = _provider.GetParticipantsExtraData(blockID, ids == null? cache : cache.Where(c => ids.Contains(c.ID)));
            return Json(new { Participants = participantsViewModel });
        }
    }
}