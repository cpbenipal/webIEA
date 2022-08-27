using System.Web;
using System.Web.Mvc;
using webIEA.App_Start;

namespace webIEA
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomErrorAttribute());
        }
    }
}
