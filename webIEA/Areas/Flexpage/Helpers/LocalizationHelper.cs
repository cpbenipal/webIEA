using Flexpage.Abstract;
using Flexpage.Helpers;
using Pluritech.Settings.Abstract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Flexpage.Helpers
{
    public class LocalizationHelper
    {
        private readonly IFlexpageSettings _settings;
        private readonly ILocalization _localization;
        public LocalizationHelper(IFlexpageSettings settings, ILocalization localization)
        {
            _settings = settings;
            _localization = localization;
        }

        public string _cookieName = "CMS_Localization-CurrentLang";

        /// <summary>
        /// Culture name if no one is specified in query string
        /// </summary>
        /// <remarks>Default value: "en"</remarks>
        public string _DefaultCultureName
        {
            get
            {
                return _settings.DefaultLangCode ?? "en";
            }
        }

        /// <summary>
        /// Parameter name for language switching
        /// </summary>
        /// <remarks>Default value: "lang"</remarks>
        public string _QueryParameterName
        {
            get
            {
                return "lang";
            }
        }

        /// <summary>
        /// Current page culture name got from QueryString
        /// </summary>
        public string _CurrentCultureName
        {
            get
            {
                return _localization.GetCurrentOrDefaultLangCode();
            }
        }

        protected virtual string getCurrentCulture()
        {
            if (HttpContext.Current != null)
            {
                HttpRequest Request = null;
                try
                {
                    Request = HttpContext.Current.Request;
                }
                catch
                {
                }

                if (Request != null)
                {
                    if (!String.IsNullOrEmpty(Request.QueryString[_QueryParameterName]))
                        return Request.QueryString[_QueryParameterName];

                    if (Request.Cookies[_cookieName] != null && !String.IsNullOrEmpty(Request.Cookies[_cookieName].Value))
                        return Request.Cookies[_cookieName].Value;

                    if (Request.UserLanguages != null && Request.UserLanguages.Length > 0)
                    {
                        List<string> languages = new List<string>();
                        foreach (string lang in Request.UserLanguages)
                        {
                            if (lang.Length >= 2)
                                languages.Add(lang.Substring(0, 2));
                        }
                        if (_settings.AllowedLanguages.Count() > 0)
                        {
                            string lang = languages.Intersect(_settings.AllowedLanguages).FirstOrDefault();
                            if (lang != null)
                                return lang;
                        }
                        else
                            return languages[0];
                    }
                }
            }
            return _DefaultCultureName;
        }

        public void SetCurrentThreadCulture()
        {
            SetCurrentThreadCulture(HttpContext.Current);
        }

        public void SetCurrentThreadCulture(HttpContext context)
        {
            string uiCulture;
            if (context.Request.QueryString[_QueryParameterName] == null
                || context.Request.QueryString[_QueryParameterName].ToString().Length == 0)
            {
                uiCulture = getCurrentCulture();
            }
            else
            {
                uiCulture = context.Request.QueryString[_QueryParameterName].ToString();
            }

            SetCurrentThreadCulture(uiCulture);
        }

        public void SetCurrentThreadCulture(string uiCulture)
        {
            try
            {
                CultureInfo info = CultureInfo.GetCultureInfo(uiCulture);
            }
            catch (ArgumentException)
            {
                uiCulture = _DefaultCultureName;
            }
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(uiCulture);

            string culture = HttpContext.GetGlobalResourceObject(_ResourceClass, _ResourceKey) as string;
            if (culture != null)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            }
        }

        private HttpCookie getLangCookie(string lang)
        {
            HttpCookie cookie = new HttpCookie(_cookieName, lang);
            cookie.Expires = DateTime.Now.AddMonths(1);
            cookie.Path = "/";
            return cookie;
        }

        /// <summary>
        /// Global resource class name for getting full culture name (ex. en-US). Class must exist!
        /// </summary>
        /// <remarks>Default value: "CultureDependent"</remarks>
        public string _ResourceClass
        {
            get
            {
                return "CultureDependent";
            }
        }

        /// <summary>
        /// Resource key for getting full culture name (ex. en-US). Key must exist!
        /// </summary>
        /// <remarks>Default value: "FullCultureName"</remarks>
        public string _ResourceKey
        {
            get
            {
                return "FullCultureName";
            }
        }

        /// <summary>
        /// Redirects page to another culture
        /// </summary>
        /// <param name="newCultureName">Culture name to be redirected</param>
        public void SwitchCulture(string newCultureName, HttpRequest Request, HttpResponse Response)
        {
            if (SaveLanguage &&
                (Request.Cookies[_cookieName] == null || Request.Cookies[_cookieName].Value != newCultureName))
            {
                if (Request.Cookies[_cookieName] == null)
                {
                    Response.Cookies.Add(getLangCookie(newCultureName));
                }
                else
                {
                    Response.Cookies[_cookieName].Value = newCultureName;
                    Response.Cookies[_cookieName].Expires = DateTime.Now.AddMonths(1);
                }
            }
            Response.Redirect(
                updateRequestQueryParameter(_QueryParameterName, newCultureName));
        }

        private string updateRequestQueryParameter(string parameter, string newValue)
        {
            return UrlHelper.UpdateUrlQueryParameter(UrlHelper.GetRawUri().OriginalString, parameter, newValue);
        }

        public string UpdateCurrentUrlToSpecificCulture(string culture)
        {
            return UrlHelper.UpdateUrlQueryParameter(UrlHelper.GetRawUri().OriginalString, _QueryParameterName, culture);
        }

        public string AppendCurrentCultureToUrl(string url)
        {
            return UrlHelper.UpdateUrlQueryParameter(url, _QueryParameterName, _CurrentCultureName);
        }

        protected bool SaveLanguage
        {
            get
            {
                return _settings.SaveCurrentLanguage;
            }
        }

        public void SaveLanguageToCookies(HttpRequest Request, HttpResponse Response)
        {
            if (SaveLanguage)
            {
                string lang = _CurrentCultureName;
                if (Request.Cookies[_cookieName] != null)
                {
                    if (Request.Cookies[_cookieName].Value != lang)
                    {
                        Response.Cookies[_cookieName].Value = lang;
                        Response.Cookies[_cookieName].Expires = DateTime.Now.AddMonths(1);
                    }
                }
                else
                {
                    Response.Cookies.Add(getLangCookie(lang));
                }
            }
        }

        public Uri GetCurrentUrlBase()
        {
            string url = UrlHelper.GetRawUri().OriginalString;
            Regex regex = new Regex(@"&?(lang=[a-z]*){1}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return new Uri(regex.Replace(url, "").TrimEnd('?'));
        }

        //public string RenderLanguageHyperlink(string culture)
        //{
        //    LocalizedPage page = HttpContext.Current.Handler as LocalizedPage;
        //    if (page == null)
        //        throw new Exception("Pages must inherit from Pluritech.WebTools.LocalizedPage in order to using localization");

        //    return String.Format("<a href='{0}'{1}>{2}</a>",
        //        page.UpdateCurrentUrlToSpecificCulture(culture),
        //        page._CurrentCultureName.Equals(culture, StringComparison.OrdinalIgnoreCase) ? " class='active'" : "",
        //        culture);
        //}
    }
}