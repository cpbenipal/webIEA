using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Helpers
{
    [Obsolete]
    public class ResourceHelper
    {
        /// <summary>
        /// Returns value from resourse file
        /// </summary>
        /// <param name="fileName">Name of resource file</param>
        /// <param name="faleValue">Value from resource file</param>
        /// <returns></returns>
        public static string GetResourseValue(string fileName, string faleValue)
        {
            return HttpContext.GetGlobalResourceObject(fileName, faleValue, System.Globalization.CultureInfo.CurrentUICulture).ToString();
        }
    }
}