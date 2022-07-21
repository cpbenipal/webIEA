using Flexpage.Code.CMS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Flexpage.Abstract.DTO;

namespace Flexpage.Models
{
    public class LocalizedStringModel : ViewModel, IComparable, ILocalizedStringModel
    {
        [JsonIgnore]
        public bool ReadOnly { get; set; } = false;
        [JsonIgnore]
        public string CurrentLangCode { get; set; }
        [JsonIgnore]
        public string CurrentLocalization { get; set; }
        [JsonIgnore]
        public string Current
        {
            get
            {
                if (CurrentLocalization == null)
                {
                    CurrentLocalization = GetCurrentLocalizaion();
                    // SetCurrentLocalisation();
                }
                return CurrentLocalization;
            }
            set
            {
                CurrentLocalization = value;
                Update();
            }
        }

        public Dictionary<string, string> Localizations { get; set; }

        [JsonIgnore]
        public string NotEmptyLocalization
        {
            get
            {
                //try to find first not empty string
                if(string.IsNullOrEmpty(CurrentLocalization))
                {
                    //if (Localizations.ContainsKey(CmsSettings.DefaultLangCode) && !string.IsNullOrEmpty(Localizations[CmsSettings.DefaultLangCode]))
                    //    return Localizations[CmsSettings.DefaultLangCode];
                    var lc = _settings.GetCurrentOrDefaultLangCode();
                    if (Localizations.ContainsKey(lc) && !string.IsNullOrEmpty(Localizations[lc]))
                        return Localizations[lc];
                    foreach (string key in Localizations.Keys)
                    {
                        if(!string.IsNullOrEmpty(Localizations[key]))
                            return Localizations[key];
                    }
                }
                return CurrentLocalization;
            }
        }

        public LocalizedStringModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) :base(settings, flexpage)
        {
            this.setLocaLocalizations(true);
        }

        public LocalizedStringModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, bool populate = false) : base(settings, flexpage)
        {
            this.setLocaLocalizations(populate);
        }

        public LocalizedStringModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, string defaultValue) : base(settings, flexpage)
        {
            this.setLocaLocalizations(true, defaultValue);
        }

        /// <summary>
        /// Returns current localization or fallbacks as ordinal (current text -> default language text -> any language text).
        /// </summary>
        /// <returns>Current localization or fallback</returns>
        public string GetCurrentLocalizaion()
        {
            if (!string.IsNullOrEmpty(CurrentLangCode))
            {
                if (Localizations.ContainsKey(CurrentLangCode))
                    return Localizations[CurrentLangCode];
                else
                    if (Settings != null && Localizations.ContainsKey(Settings.DefaultLangCode))
                        return Localizations[Settings.DefaultLangCode];
            }
            if (String.IsNullOrEmpty(CurrentLocalization))
                return Localizations.Values.FirstOrDefault(e => !String.IsNullOrEmpty(e));
            return null;
        }

        public void SetCurrentLocalisation()
        {
            if (!string.IsNullOrEmpty(CurrentLangCode))
                CurrentLocalization = Localizations.ContainsKey(CurrentLangCode) ? Localizations[CurrentLangCode] : "";            
        }

        /// <summary>
        /// Adds new localization
        /// </summary>
        /// <param name="langCode">Language code</param>
        public void AddLanguage(string langCode, string value)
        {
            Localizations.Add(langCode, (string.IsNullOrEmpty(value) ? string.Empty : value));
            SetCurrentLocalisation();
        }

        public void AddText(string lang, string text)
        {
            Localizations[lang] = text;
            SetCurrentLocalisation();
        }

        /// <summary>
        /// Sets new localization
        /// </summary>
        /// <param name="langCode">Language code</param>
        public void SelectLanguage(string langCode)
        {
            if (!Localizations.ContainsKey(langCode))
                AddLanguage(langCode,string.Empty);

            CurrentLangCode = langCode;
            SetCurrentLocalisation();
        }

        public void SelectLanguageAndSetValue(string langCode)
        {
            if (!Localizations.ContainsKey(langCode))
            {
                if (Localizations.ContainsKey(_settings.GetCurrentOrDefaultLangCode()))
                {
                    AddLanguage(langCode, Localizations[_settings.GetCurrentOrDefaultLangCode()]);
                }
                else if (Localizations.ContainsKey(_settings.DefaultLangCode))
                {
                    AddLanguage(langCode, Localizations[_settings.DefaultLangCode]);
                }
                else
                {
                    var value = Localizations.Select(s => s.Value).FirstOrDefault(p => !string.IsNullOrEmpty(p));

                    if (string.IsNullOrEmpty(value))
                        return;

                    AddLanguage(langCode, value);
                }
            }
                

            CurrentLangCode = langCode;
            SetCurrentLocalisation();
        }
        /// <summary>
        /// Fills page model with data
        /// </summary>
        /// <param name="source">Dictionary with values</param>
        /// <param name="args">Should be empty</param>
        public override void Assign(object source, params object[] args)
        {
            base.Assign(source);
            Localizations = source as Dictionary<string, string>;
            SetCurrentLocalisation();
        }

        public LocalizedStringModel(): base(null, null) 
        {
            Localizations = new Dictionary<string, string>();
        }

        public LocalizedStringModel(string text, Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : this(settings, flexpage)
        {
            try
            {
                var l = FromJSON<Dictionary<string, string>>(text);
                if (l != null)
                    Localizations = l;
                else
                {
                    Localizations = new Dictionary<string, string>();
                    if (settings != null)
                        Localizations.Add(settings.DefaultLangCode, text);
                }
                    
            }
            catch
            {
                Localizations = new Dictionary<string, string>() { { settings.DefaultLangCode, text } };
            }

            SelectLanguage(_settings.GetCurrentOrDefaultLangCode());
        }

        public static LocalizedStringModel CreateNew(string text, Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage)
        {
            LocalizedStringModel r = null;
            try
            {
                r = FromJSON<LocalizedStringModel>(text);
                if (r != null)
                {
                    r._settings = settings;
                    r.SelectLanguage(settings.DefaultLangCode);
                }
            }
            catch
            {
            }

            if (r == null)
            {
                r = new LocalizedStringModel(settings, flexpage) { Localizations = new Dictionary<string, string>() { { settings.DefaultLangCode, text ?? "" } } };
                r.SelectLanguage(settings.DefaultLangCode);
            }

            return r;
        }

        public static explicit operator string(LocalizedStringModel source)
        {
            return ToJSON(source.Localizations);
        }

        public override void Update()
        {
            Localizations[CurrentLangCode] = CurrentLocalization;
        }

        public string GetText(string lang)
        {
            if(Localizations.ContainsKey(lang))
                return Localizations[lang];
            return null;
        }

        public string GetText()
        {
            return string.IsNullOrEmpty(CurrentLocalization)
                ? Localizations.FirstOrDefault(p => !string.IsNullOrEmpty(p.Value)).Value
                : CurrentLocalization;
        }

        #region Helpers

        private void setLocaLocalizations(bool populate, string value = "")
        {
            Localizations = new Dictionary<string, string>();
            CurrentLangCode = _settings.GetCurrentOrDefaultLangCode();
            CurrentLocalization = value;
            if (populate)
                Localizations.Add(CurrentLangCode, value);
        }

        public int CompareTo(object obj)
        {
            if (obj is LocalizedStringModel)
            {
                var m = obj as LocalizedStringModel;
                return string.Compare(Current, m.Current);
            }
            return 0;
        }

        public LocalizedStringModel Truncate(int maxLength)
        {
            var r = new LocalizedStringModel();
            var s = ToJson();
            int d = s.Length - maxLength;
            if (d > 0 && maxLength > 0)
            {
                double k = maxLength / s.Length;
                foreach (var p in Localizations)
                {
                    int l = (int)Math.Round(p.Value.Length * k) - 1;
                    r.Localizations.Add(p.Key, p.Value.Substring(0, l));
                }
            }
            else
            {
                r.Localizations = new Dictionary<string, string>(Localizations);
            }
            return r;
        }

        #endregion
    }
}