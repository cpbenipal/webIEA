using System.Collections.Generic;

namespace Flexpage.Models
{ 
    public class GeneralPermissionsModel : ViewModel
    {
        public List<ListPermissionsModel> ListPermissionsModel { get; set; }

        public string JsonDataSave { get; set; }

        public GeneralPermissionsModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            ListPermissionsModel = new List<ListPermissionsModel>();
        }
    }
}