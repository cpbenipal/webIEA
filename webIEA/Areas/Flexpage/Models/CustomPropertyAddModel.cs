using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using FlexPage2.Areas.Flexpage.Models.CustomPropertyAddModel;
using Newtonsoft.Json;

namespace Flexpage.Models
{

    public class CustomPropertyAddModel : ViewModel
    {
        public int SelectId { get; set; }

        public ObjectPropertiesRequest PropertiesRequest { get; set; }

        public string PropertiesRequestJson { get; set; }

        public List<SelectListItem> SelectListItems { get; set; }

        public CustomPropertyAddModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            PropertiesRequest = new ObjectPropertiesRequest();
            SelectListItems = new List<SelectListItem>();
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title, bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);

            if (string.IsNullOrEmpty(proto.Parameters))
                return;

            ConvertData(proto.Parameters);
        }

        public void ConvertData(string json)
        {
            PropertiesRequestJson = json;
            PropertiesRequest = JsonConvert.DeserializeObject<ObjectPropertiesRequest>(json);
        }
    }
}