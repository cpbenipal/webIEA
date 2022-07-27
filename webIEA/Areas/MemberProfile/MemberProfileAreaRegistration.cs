using System.Web.Mvc;

namespace webIEA.Areas.MemberProfile
{
    public class MemberProfileAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MemberProfile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MemberProfile_default",
                "MemberProfile/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );             
        }
    }
}