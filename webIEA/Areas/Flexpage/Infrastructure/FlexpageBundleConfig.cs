using System.Web.Optimization;

namespace Flexpage.Area
{
    public static class FlexpageBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/Flexpage/jquery").Include(
                        "~/Areas/Flexpage/Content/Scripts/bootstrap/jquery-{version}.js",
                        "~/Areas/Flexpage/Content/Scripts/bootstrap/jquery-ui-{version}.js",
                        "~/Areas/Flexpage/Content/Scripts/bootstrap/jquery.easing.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/jqueryval").Include(
                         "~/Areas/Flexpage/Content/Scripts/bootstrap/jquery.validate.min.js",
                        "~/Areas/Flexpage/Content/Scripts/bootstrap/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/jqueryunob").Include(
                        "~/Areas/Flexpage/Content/Scripts/bootstrap/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/cookie-manager").Include(
                 "~/Areas/Flexpage/Content/Scripts/bootstrap/cookie-manager.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/Flexpage/modernizr").Include(
                        "~/Areas/Flexpage/Content/Scripts/bootstrap/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/bootstrap").Include(
                      "~/Areas/Flexpage/Content/Scripts/bootstrap/bootstrap.min.js",
                      "~/Areas/Flexpage/Content/Scripts/bootstrap/bootstrap-select.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/moment").Include(
                        "~/Areas/Flexpage/Content/Scripts/moment.min.js",
                        "~/Areas/Flexpage/Content/Scripts/moment-with-locales.min"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/popper").Include(
                     "~/Areas/Flexpage/Content/Scripts/bootstrap/umd/popper.min.js",
                     "~/Areas/Flexpage/Content/Scripts/bootstrap/umd/popper-utils.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/respond").Include(
                    "~/Areas/Flexpage/Content/Scripts/bootstrap/respond.min.js",
                    "~/Areas/Flexpage/Content/Scripts/bootstrap/respond.matchmedia.addListener.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/dropzone").Include(
               "~/Areas/Flexpage/Content/Scripts/bootstrap/dropzone.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/colorpicker").Include(
              "~/Areas/Flexpage/Content/Scripts/bootstrap/bootstrap-colorpicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/datepicker").Include(
              "~/Areas/Flexpage/Content/Scripts/bootstrap/bootstrap-datepicker.min.js",
              "~/Areas/Flexpage/Content/Scripts/bootstrap/locales/bootstrap-datepicker.fr.min.js",
              "~/Areas/Flexpage/Content/Scripts/bootstrap/locales/bootstrap-datepicker.nl.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Flexpage/keyframes").Include(
              "~/Areas/Flexpage/Content/Scripts/bootstrap/jquery.keyframes.min.js"));

            bundles.Add(new StyleBundle("~/bundles/Flexpage/bootstrap/css").Include(
                "~/Areas/Flexpage/Content/css/bootstrap.css",
                "~/Areas/Flexpage/Content/css/bootstrap-theme.css",
                "~/Areas/Flexpage/Content/css/bootstrap-select.css",
                "~/Areas/Flexpage/Content/css/Bootstra4PaddingMarginSettings.css",
                //"~/Content/css/bootstrap-datepicker3.min.css",    // no such file in the basic Flexpage
                "~/Areas/Flexpage/Content/css/jquery.ez-plus.css",
                "~/Areas/Flexpage/Content/css/site.css").Include(
                "~/Areas/Flexpage/Content/css/font-awesome.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/bundles/Flexpage/Content/themes/base/").Include(
                "~/Areas/Flexpage/Content/themes/base/jquery-ui.css"));

            //These packages must be used in the site layout.
            bundles.Add(new StyleBundle("~/bundles/Flexpage/Content/css").Include(
                "~/Areas/Flexpage/Content/bootstrap/flexpage.content.style.css",
                "~/Areas/Flexpage/Content/Frontend/flexpage.css",
                "~/Areas/Flexpage/Content/Frontend/PageNotFound.css"
            ));

            //Use these bundles only in admin layout.
            bundles.Add(new StyleBundle("~/bundles/Flexpage/css")
                .Include(
                     "~/Areas/Flexpage/Content/bootstrap/bootstrap-colorpicker.min.css",
                     "~/Areas/Flexpage/Content/bootstrap/flexpage-style.css",
                     "~/Areas/Flexpage/Content/bootstrap/theme/bootstrap-theme.css",
                     "~/Areas/Flexpage/Content/bootstrap/flexpage.content.style.css",
                     "~/Areas/Flexpage/Content/flexpage.css"
                 )
                .Include(
                    "~/Areas/Flexpage/Content/css/font-awesome.css", new CssRewriteUrlTransform()
                ));

            //Use these bundles only in admin layout. "~/Areas/Flexpage/Content/Scripts/Popup.js",
            bundles.Add(new ScriptBundle("~/Flexpage/js").Include(
                "~/Areas/Flexpage/Content/Scripts/FlexPageContants.js",
                "~/Areas/Flexpage/Content/Scripts/HtmlEditor.js",
                "~/Areas/Flexpage/Content/Scripts/SitemapProvider.js",
                "~/Areas/Flexpage/Content/Scripts/jquery.unsaved-data-tracker.js"
           ));

            //These packages must be used in the administrator's layout and site layout.
            bundles.Add(new ScriptBundle("~/Flexpage/Content/js").Include(
                //"~/Areas/Flexpage/Content/Scripts/FacebookSDKConnector.js",
                //"~/Areas/Flexpage/Content/Scripts/TwitterWidgets.js",
                "~/Areas/Flexpage/Content/Scripts/Popup-content.js",
                //"~/Areas/Flexpage/Content/Scripts/Menu.js",
                "~/Areas/Flexpage/Content/Scripts/IEPolyfill.js",
                "~/Areas/Flexpage/Content/Scripts/Flexpage.es5.js",
                //"~/Areas/Flexpage/Content/Scripts/VideoPlaylist.js",
                //"~/Areas/Flexpage/Content/Scripts/WebForm.js",
                "~/Areas/Flexpage/Content/Scripts/pdfPreview.js",
                "~/Areas/Flexpage/Content/Scripts/Popup.js",
                //"~/Areas/Flexpage/Content/Scripts/TabContainer.js",
                "~/Areas/Flexpage/Content/Scripts/FlexpageCalendar.js",
                //"~/Areas/Flexpage/Content/Scripts/calendar/fullcalendar.min.js",
                //"~/Areas/Flexpage/Content/Scripts/calendar/locale-all.js",
                //"~/Areas/Flexpage/Content/Scripts/AdvertisementCycle.js",
                "~/Areas/Flexpage/Content/Scripts/ParticipantsCycle.js",
                //"~/Areas/Flexpage/Content/Scripts/Breadcrumbs.js",
                "~/Areas/Flexpage/Content/Scripts/CustomProperties/EditCustomProperties.js"
            ));

            // Gallery bundles
            bundles.Add(new ScriptBundle("~/Flexpage/Content/ImageGallery/js").Include(
                //"~/Areas/Flexpage/Content/Scripts/Gallery/jgallery.min.js",
                //"~/Areas/Flexpage/Content/Scripts/Gallery/jquery.justifiedGallery.min.js",
                //"~/Areas/Flexpage/Content/Scripts/Gallery/jquery.magnific-popup.min.js",
                //"~/Areas/Flexpage/Content/Scripts/Gallery/photoswipe-ui-default.min.js",
                //"~/Areas/Flexpage/Content/Scripts/Gallery/easyzoom.js",
                //"~/Areas/Flexpage/Content/Scripts/Gallery/jquery.ez-plus.js",
                //"~/Areas/Flexpage/Content/Scripts/Gallery/jquery.simplePagination.js",
                //"~/Areas/Flexpage/Content/Scripts/Gallery/Common.js",
                //"~/Areas/Flexpage/Content/Scripts/Gallery/GalleryFlexpage.js"
                ));

            bundles.Add(new StyleBundle("~/bundles/Flexpage/coockestyle").Include(
                "~/Areas/Flexpage/Content/css/cookiecuttr.css"));

            bundles.Add(new StyleBundle("~/bundles/Flexpage/coockescript").Include(
                "~/Areas/Flexpage/Content/Scripts/jquery.cookiecuttr.js"));

            bundles.Add(new ScriptBundle("~/Flexpage/GeneralPermissionService").Include(
                "~/Areas/Flexpage/Content/Scripts/GeneralPermissionService.js"
            ));

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}