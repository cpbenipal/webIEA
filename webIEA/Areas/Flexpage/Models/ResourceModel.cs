using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class ResourceModel
    {
        public string ResourceFileName { get; set; } = "Resource";

        /// <summary>
        /// Returns value from resourse file
        /// </summary>
        /// <param name="faleValue">Value from resource file</param>
        /// <returns></returns>
        protected string GetResourseValue(string faleValue)
        {
            return HttpContext.GetGlobalResourceObject(ResourceFileName, faleValue, System.Globalization.CultureInfo.CurrentUICulture).ToString();
        }
    }
}