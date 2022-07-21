using System.Collections.Generic;

namespace FlexPage2.Areas.Flexpage.Models.CustomPropertyAddModel
{
    public class ObjectPropertiesRequest
    {
        public int ObjectId { get; set; }

        public List<ObjectPropertiesContent> Properties { get; set; } = new List<ObjectPropertiesContent>();
    }
}