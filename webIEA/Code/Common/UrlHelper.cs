using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Flexpage.Code.Common
{
    public static class UrlHelper
    {
        public static string GetVirtualPath(string physicalPath)
        {
            if(!physicalPath.StartsWith(HttpContext.Current.Request.PhysicalApplicationPath))
            {
                throw new InvalidOperationException("Physical path is not within the application root");
            }

            return "/" + physicalPath.Substring(HttpContext.Current.Request.PhysicalApplicationPath.Length)
                  .Replace("\\", "/");
        }

        public static string UpdateUrlQueryParameter(string parameter, string newValue)
        {
            return UpdateUrlQueryParameter(GetRawUri().PathAndQuery, parameter, newValue);
        }

        public static string UpdateUrlQueryParameter(string url, string parameter, string newValue)
        {
            int idxAmp = url.IndexOf("&" + parameter);
            int idxQue = url.IndexOf("?" + parameter);
            if(idxAmp == -1 && idxQue == -1)
            {
                url += String.Format("{0}{1}={2}", (url.IndexOf("?") == -1) ? "?" : "&",
                    parameter, newValue);
            }
            else
            {
                string prefix = idxQue == -1 ? "&" : "?";
                int idx = idxQue == -1 ? idxAmp : idxQue;
                int idxNextAmp = url.IndexOfAny("&#".ToCharArray(), idx + 1);
                if(idxNextAmp == -1)
                {
                    url = url.Remove(idx);
                    url += String.Format("{0}{1}={2}", (url.IndexOf("?") == -1) ? "?" : "&",
                        parameter, newValue);
                }
                else
                {
                    url = url.Replace(prefix + url.Substring(idx + 1, idxNextAmp - idx - 1),
                        prefix + parameter + "=" + newValue);
                }

            }
            return url;
        }

        public static string RemoveUrlQueryParameter(string url, string parameter)
        {
            Regex re = new Regex(
                String.Format("(([\\?&]){0}=[^&\\?]*&)|(([\\?&]){0}=[^&\\?]*$)", parameter), 
                RegexOptions.IgnoreCase);
            string s = url;
            if(re.IsMatch(url))
            {
                s = re.Replace(url, "$2");
            }
            return s;
        }

        /// <summary>
        /// Returns current URL, paying attention to URL rewriting
        /// </summary>
        public static Uri GetRawUri()
        {
            string rawUrl = HttpContext.Current.Request.RawUrl;
            if(rawUrl != null)
                return new Uri(HttpContext.Current.Request.Url, rawUrl);
            else
                return HttpContext.Current.Request.Url;
        }
    }
}
