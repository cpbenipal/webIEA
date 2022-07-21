using System.Collections.Generic;

namespace Flexpage.Models
{
    public class ListPermissionsModel
    {
        public int Id { get; set; }
        public string Permission { get; set; }

        public List<WhoPermissionsModel> WhoPermissionsModels { get; set; }

        public ListPermissionsModel()
        {
            WhoPermissionsModels = new List<WhoPermissionsModel>();
        }
    }
}