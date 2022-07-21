using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Flexpage.Code.CMS;
using Newtonsoft.Json;

namespace Flexpage.Helpers
{       
    public class LocalizedField
    {
        private Dictionary<string, string> strings = new Dictionary<string, string>();

        /// <summary>
        /// Boxes input array of strings into one string
        /// </summary>
        /// <param name="values">Key is language</param>
        public static string BoxValues(Dictionary<string, string> values)
        {
            const string openBr = "{";
            const string closeBr = "}";
            const string blank = "\"{0}\":\"{1}\"";
            string result = string.Empty;
            foreach(string key in values.Keys)
            {
                string val = values[key] ?? string.Empty;
                result += (string.IsNullOrEmpty(result) ? string.Empty : ",") +
                    string.Format(blank, key, val.Replace("\"", "''"));
            }
            return string.Concat(openBr, result, closeBr);
        }

        /// <summary>
        /// Returns list of language+value of input string
        /// </summary>
        public static Dictionary<string, string> UnboxValue(string value, bool appenAllLanguages)
        {
            LocalizedField lf = LocalizedField.Parse(value);
            if(appenAllLanguages)
            {
                CmsSettings.GetAllowedLanguagesList()
                    .Except(lf.strings.Select(k => k.Key)).ToList()
                    .ForEach(l => lf.strings.Add(l, string.Empty));
            }
            return lf.strings;
        }

        /// <summary>
        /// Creates localized string from input values
        /// <exception cref="AM's commment: en+fr+nl the most popular bundle of languages, so I've created separate method for it"/>
        /// </summary>
        public static string CreateLocalizedString(string engValue, string frValue, string nlValue)
        {
            return LocalizedField.BoxValues(new Dictionary<string, string>()
        {
            {"en", engValue},
            {"fr", frValue},
            {"nl", nlValue}
        });
        }

        static private LocalizedField Parse(string value)
        {
            try
            {
                Dictionary<string, string> tmp =
                    (Dictionary<string, string>)JsonConvert.DeserializeObject(value, typeof(Dictionary<string, string>));

                if(tmp == null)
                {
                    return new LocalizedField();
                }
                else
                {
                    LocalizedField lf = new LocalizedField();
                    lf.strings = tmp;
                    return lf;
                }
            }
            catch
            {
                if(string.IsNullOrEmpty(value))
                {
                    return new LocalizedField();
                }
                else
                {
                    LocalizedField lf = new LocalizedField();
                    lf.strings.Add(CmsSettings.DefaultLangCode, value);
                    return lf;
                }
            }
        }

        static public string DisplayText(string value)
        {
            return DisplayText(value, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower());
        }

        static public string DisplayTextForTabbed(string value, string language)
        {
            return DisplayText(value, language);
        }

        static public string DisplayText(string value, string lang)
        {
            LocalizedField lf = LocalizedField.Parse(value);

            if(lf.strings.Any(x => x.Key == lang))
            {
                return lf.strings[lang];
            }

            if(lf.strings.Any(x => x.Key == CmsSettings.DefaultLangCode))
            {
                return lf.strings[CmsSettings.DefaultLangCode];
            }

            return string.Empty;
        }

        static public string DisplayTextNonEmpty(string value, string lang)
        {
            LocalizedField lf = LocalizedField.Parse(value);

            if(lf.strings.Any(x => x.Key == lang))
            {
                return lf.strings[lang];
            }

            if(lf.strings.Any(x => x.Key == CmsSettings.DefaultLangCode))
            {
                return lf.strings[CmsSettings.DefaultLangCode];
            }

            if(lf.strings.Keys.Count > 0)
                return lf.strings[lf.strings.Keys.First()];

            return string.Empty;
        }
    }
}