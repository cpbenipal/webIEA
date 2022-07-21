using System.Web.Mvc;
using System.Web.Optimization;

namespace Flexpage.Area
{
    public class FlexpageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Flexpage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            RegisterRoutes(context);
        }

        private void RegisterRoutes(AreaRegistrationContext context)
        {
            context.Namespaces.Add("Flexpage.Controllers.*");

            context.MapRoute(
                "Flexpage_default",
                "Flexpage/{action}/{alias}",
                new { controller = "Flexpage", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                 name: "Flexpage_Page",
                 url: "Page/{action}/{alias}",
                 defaults: new { controller = "Page", action = "Index", alias = UrlParameter.Optional }
             );


            context.MapRoute(
                 name: "Flexpage_Block",
                 url: "Block/{action}/{alias}",
                 defaults: new { controller = "Block", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_BlockList",
                 url: "BlockList/{action}/{alias}",
                 defaults: new { controller = "BlockList", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_MenuBlock",
                 url: "MenuBlock/{action}/{alias}",
                 defaults: new { controller = "MenuBlock", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_WizardBlock",
                 url: "wizard/{action}/{alias}",
                 defaults: new { controller = "WizardBlock", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_Sitemap",
                 url: "SitemapManager/{action}/{alias}",
                 defaults: new { controller = "SitemapManager", action = "GetSitemapManagerContent", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_BreadcrumbsBlock",
                 url: "BreadcrumbsBlock/{action}/{alias}",
                 defaults: new { controller = "BreadcrumbsBlock", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                name: "Flexpage_CalendarBlock",
                url: "CalendarBlock/{action}/{alias}",
                defaults: new { controller = "CalendarBlock", action = "Calendar", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                 name: "Flexpage_NewsBlock",
                 url: "News/{action}/{alias}",
                 defaults: new { controller = "News", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_Events",
                 url: "Events/{action}/{alias}",
                 defaults: new { controller = "Events", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_Admin",
                 url: "Admin/{action}/{alias}",
                 defaults: new { controller = "Admin", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_Notifications",
                 url: "Notifications/{action}/{alias}",
                 defaults: new { controller = "Notifications", action = "Index", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_Link",
                 url: "Link/{action}/{alias}",
                 defaults: new { controller = "Link", action = "LinkManager", alias = UrlParameter.Optional }
             );

            context.MapRoute(
                 name: "Flexpage_Schedule",
                 url: "Schedule/{action}/{alias}",
                 defaults: new { controller = "Schedule", action = "LoadScheduleViewModel", alias = UrlParameter.Optional }
             );
            context.MapRoute(
                name: "Flexpage_FAQBlock",
                url: "FAQBlock/{action}/{alias}",
                defaults: new { controller = "FAQBlock", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_CmsTextBlock",
                url: "CmsTextBlock/{action}/{alias}",
                defaults: new { controller = "CmsTextBlock", action = "Index", alias = UrlParameter.Optional }
            );
            context.MapRoute(
                 name: "Flexpage_CSSSettings",
                 url: "CSSSettings/{action}/{alias}",
                 defaults: new { controller = "CSSSettings", action = "CSSSettings", alias = UrlParameter.Optional }
             );
            context.MapRoute(
                name: "Flexpage_WebFormBlock",
                url: "WebFormBlock/{action}/{alias}",
                defaults: new { controller = "WebFormBlock", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_NavigationBar",
                url: "NavigationBar/{action}/{alias}",
                defaults: new { controller = "NavigationBar", action = "NavigationBar", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_Websites",
                url: "Websites/{action}/{id}",
                defaults: new { controller = "Websites", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_Participants",
                url: "ParticipantsBlock/{action}/{id}",
                defaults: new { controller = "ParticipantsBlock", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_PictureBlock",
                url: "PictureBlock/{action}/{alias}",
                defaults: new { controller = "PictureBlock", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_Layout",
                url: "Layout/{action}/{alias}",
                defaults: new { controller = "Layout", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_Session",
                url: "Session/{action}/{alias}",
                defaults: new { controller = "Session", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_Settings",
                url: "Settings/{action}/{alias}",
                defaults: new { controller = "Settings", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_ContactProperties",
                url: "ContactProperties/{action}/{alias}",
                defaults: new { controller = "ContactProperties", action = "Index", alias = UrlParameter.Optional }
            );
            context.MapRoute(
                name: "Flexpage_ContactsEnumeration",
                url: "ContactsEnumeration/{action}/{alias}",
                defaults: new { controller = "ContactsEnumeration", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_ContactDetails",
                url: "ContactDetails/{action}/{alias}",
                defaults: new { controller = "ContactDetails", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_ContactsBrowser",
                url: "ContactsBrowser/{action}/{alias}",
                defaults: new { controller = "ContactsBrowser", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_AdvancedSearch",
                url: "AdvancedSearch/{action}/{alias}",
                defaults: new { controller = "AdvancedSearch", action = "Index", alias = UrlParameter.Optional }
            );

            context.MapRoute(
                name: "Flexpage_Export",
                url: "Export/{action}/{alias}",
                defaults: new { controller = "Export", action = "Index", alias = UrlParameter.Optional }
            );
        }

        private void RegisterBundles()
        {
            FlexpageBundleConfig.RegisterBundles(BundleTable.Bundles);
        }

    }
}