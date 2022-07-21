using Flexpage.Code.CMS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class LocalizedEnumValueModel : LocalizedStringModel
    {

        public static LocalizedEnumValueModel CreateNew(string text, Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage)
        {
            LocalizedEnumValueModel res = null;
            try
            {
                res = FromJSON<LocalizedEnumValueModel>(text);
                res._settings = settings;
                res.SelectLanguage(res.Localizations.Keys.FirstOrDefault());
            }
            catch
            {
                res = new LocalizedEnumValueModel(settings, flexpage);
            }
            return res;
        }

        public int Value { get; set; }

        public LocalizedEnumValueModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            this.SetLoaLocalizations(true);
        }

        public LocalizedEnumValueModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, bool populate = false) : base(settings, flexpage)
        {
            this.SetLoaLocalizations(populate);
        }

        public LocalizedEnumValueModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, string defaultValue) : base(settings, flexpage)
        {
            this.SetLoaLocalizations(true, defaultValue);
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

        public LocalizedEnumValueModel() : base(null, null)
        {
            Localizations = new Dictionary<string, string>();
        }

        public static explicit operator string(LocalizedEnumValueModel source)
        {
            return ToJSON(source.Localizations);
        }

        public override void Update()
        {
            Localizations[CurrentLangCode] = CurrentLocalization;
        }

        public string GetText(string lang)
        {
            if (Localizations.ContainsKey(lang))
                return Localizations[lang];
            return null;
        }

        #region Helpers

        private void SetLoaLocalizations(bool populate, string value = "")
        {
            Localizations = new Dictionary<string, string>();
            CurrentLangCode = _settings.GetCurrentOrDefaultLangCode();
            CurrentLocalization = value;
            if (populate)
                Localizations.Add(CurrentLangCode, value);
        }

        #endregion
    }
}