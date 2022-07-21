using DevExpress.Data;
using Flexpage.Domain.Abstract;
using Flexpage.Helpers;
using System.Web.Mvc;

namespace Flexpage.Models
{
    
    public class CustomPropertiesEditTemplateHtmlModel
    {
        [AllowHtml]
        public string Value { get; set; }
        public int ObjectPropertyID { get; set; }
        public int PropertyID { get; set; }
        public string Name { get; set; } = "CB_Editor_Html";
        public CustomPropertiesEditTemplateHtmlModel()
        {
          
        }
      
    }

}