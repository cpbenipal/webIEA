using System.Web.Mvc;

namespace webIEA.Areas.Providers
{
    public class IEAProvidersAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "IEAProviders";
            } 
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "IEAProviders_default",
                "IEAProviders/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}