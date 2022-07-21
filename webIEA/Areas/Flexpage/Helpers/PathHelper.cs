

using System.Web.Configuration;

namespace FlexPage2.Areas.Flexpage.Helpers
{
    public class PathHelper
    {
        // Banners editor using for all pages, so it has 1 alias for now
        private static string bannersAllias = "BannersAlias";

        public static string GetBannerPath(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return "";
            }

            return GetBannerFolder() + imageName;
        }


        public static string GetBannerFolder()
        {
            return WebConfigurationManager.AppSettings["TempUploadedImagesPath"] + bannersAllias + "/";
        }
    }
}