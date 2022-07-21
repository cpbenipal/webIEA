using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlexPage2.Areas.Flexpage.Models.CustomPropertyAddModel
{
    public class ObjectPropertiesContent
    {
        public int Id { get; set; }

        public object Value { get; set; }

        public StatusPropertyEnum Status { get; set; }
    }
}