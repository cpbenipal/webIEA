using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;
using System.Globalization;

namespace Flexpage.Code.CMS
{
    [Obsolete("This class should be removed. User protected _settings property of a coltroller/model.")]
    public static class CmsSettings
    {
        public static string DefaultLangCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CMS:DefaultLangCode"] ?? String.Empty;
            }
        }

        public static string AllowedLanguages
        {
            get
            {
                return ConfigurationManager.AppSettings["CMS:AllowedLanguages"] ?? String.Empty;
            }
        }
        public static bool ScrambleEmails
        {
            get
            {
                bool val;
                if(Boolean.TryParse(ConfigurationManager.AppSettings["CMS:ScrambleEmails"], out val))
                {
                    return val;
                }

                return true;
            }
        }
        public static bool UseWYSIWYGEditor
        {
            get
            {
                bool val;
                if(Boolean.TryParse(ConfigurationManager.AppSettings["CMS:UseWYSIWYGEditor"], out val))
                {
                    return val;
                }
                return true;
            }
        }

        public static bool DialogBasedEditing
        {
            get
            {
                bool val;
                if(Boolean.TryParse(ConfigurationManager.AppSettings["CMS:DialogBasedEditing"], out val))
                {
                    return val;
                }

                return false;
            }
        }

        public static List<string> GetAllowedLanguagesList()
        {
            return new List<String>(AllowedLanguages.ToLowerInvariant().Split(',', ';', ' '));

        }
        public static string GetCurrentOrDefaultLangCode()
        {
            string current = CurrentUiCulture.Name.ToLower();
            if(!GetAllowedLanguagesList().Contains(current) && !StrictLanguage)
            {
                current = DefaultLangCode;
            }
            return current;
        }

        public static CultureInfo CurrentUiCulture
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
        }

        public static bool StrictLanguage
        {
            get
            {
                return true.ToString().Equals(
                    ConfigurationManager.AppSettings["CMS:StrictLanguage"],
                    StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static string ImageBrowserURL
        {
            get
            {
                return ConfigurationManager.AppSettings["CMS:ImageBrowserURL"] ?? string.Empty;
            }
        }

        public static string PrototypePath
        {
            get
            {
                return ConfigurationManager.AppSettings["CMS:PrototypePath"] ?? "js/prototype.js";
            }
        }

        public static string FilterPath
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:FilterPath"] ?? "js/FilterManager.js";
            }
        }

        public static string MulticolumnPath
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:MulticolumnPath"] ?? "js/MultiColumn.js";
            }
        }

        public static string GalleryPath
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:GalleryPath"] ?? "js/gallery.js";
            }
        }

        public static string WebFormPath
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:WebFormPath"] ?? "WebForm.js";
            }
        }

        public static string JqueryPath
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:JqueryPath"] ?? "js/jquery.js";
            }
        }

        public static string JqueryUIPath
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:JqueryUIPath"] ?? "js/jquery.ui.js";
            }
        }

        public static string JqueryUICSSPath
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:JqueryUICssPath"] ?? "css/jquery-ui.css";
            }
        }

        public static string JqueryMigrate
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:JqueryMerge"] ?? "js/jquery-migrate.js";
            }
        }

        public static string JqueryPathPrevious
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:JqueryPathPrevious"] ?? "js/jquery.pre.js";
            }
        }

        public static string JqueryUIPathPrevious
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:JqueryUIPathPrevious"] ?? "js/jquery.ui.pre.js";
            }
        }

        public static string JqueryMigratePrevious
        {
            get
            {
                return ConfigurationManager.AppSettings["JS:JqueryMergePrevious"] ?? "js/jquery-migrate.pre.js";
            }
        }
        public static bool IsAdmin()
        {
            bool adminMode = false;
            if(!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CMS:AdminRoles"]))
            {
                foreach(string role in
                    ConfigurationManager.AppSettings["CMS:AdminRoles"].Split(',', ';'))
                {
                    if(HttpContext.Current.Request.IsAuthenticated
                        && HttpContext.Current.User.IsInRole(role))
                    {
                        adminMode = true;
                        break;
                    }
                }
            }
            return adminMode;
        }
    }
}