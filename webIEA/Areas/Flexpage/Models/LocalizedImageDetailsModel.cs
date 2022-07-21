using System.Collections.Generic;
using System.Linq;

namespace Flexpage.Models
{
    public class LocalizedImageDetailsModel : ViewModel
    {
        public string ChangeLangFunc { get; set; }

        public LocalizedStringModel LocalizedTitle { get; set; }
        public LocalizedStringModel LocalizedDescription { get; set; }
        public LocalizedStringModel LocalizedAlternateText { get; set; }
        public LocalizedStringModel LocalizedLinkUrl { get; set; }

        public List<string> AllKeys
        {
            get
            {
                return LocalizedTitle.Localizations.Keys
                            .Union(LocalizedAlternateText.Localizations.Keys
                                .Union(LocalizedDescription.Localizations.Keys
                                    .Union(LocalizedLinkUrl.Localizations.Keys)))
                            .Distinct()
                            .ToList();
            }
        }

        public List<string> AllFilledKeys
        {
            get
            {
                HashSet<string> descrLangs = new HashSet<string>();
                foreach (string key in LocalizedTitle.Localizations.Keys)
                {
                    if (!string.IsNullOrWhiteSpace(LocalizedTitle.Localizations[key]))
                    {
                        descrLangs.Add(key);
                    }
                }
                foreach (string key in LocalizedAlternateText.Localizations.Keys.Except(descrLangs).ToList())
                {
                    if (!string.IsNullOrWhiteSpace(LocalizedAlternateText.Localizations[key]))
                    {
                        descrLangs.Add(key);
                    }
                }
                foreach (string key in LocalizedDescription.Localizations.Keys.Except(descrLangs).ToList())
                {
                    if (!string.IsNullOrWhiteSpace(LocalizedDescription.Localizations[key]))
                    {
                        descrLangs.Add(key);
                    }
                }
                foreach (string key in LocalizedLinkUrl.Localizations.Keys.Except(descrLangs).ToList())
                {
                    if (!string.IsNullOrWhiteSpace(LocalizedLinkUrl.Localizations[key]))
                    {
                        descrLangs.Add(key);
                    }
                }
                return descrLangs.ToList();
            }
        }

        public LocalizedImageDetailsModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) :base(settings, flexpage)
        {
            LocalizedTitle = new LocalizedStringModel(settings, flexpage);
            LocalizedAlternateText = new LocalizedStringModel(settings, flexpage);
            LocalizedLinkUrl = new LocalizedStringModel(settings, flexpage);
            LocalizedDescription = new LocalizedStringModel(settings, flexpage);

            LocalizedAlternateText.CurrentLangCode = 
                LocalizedDescription.CurrentLangCode = 
                LocalizedLinkUrl.CurrentLangCode = 
                LocalizedTitle.CurrentLangCode = 
                _settings.GetCurrentOrDefaultLangCode();
        }

        public void SetCurrentLocalisation()
        {
            LocalizedAlternateText.SetCurrentLocalisation();
            LocalizedDescription.SetCurrentLocalisation();
            LocalizedLinkUrl.SetCurrentLocalisation();
            LocalizedTitle.SetCurrentLocalisation();
        }

        /// <summary>
        /// Sets new localization
        /// </summary>
        /// <param name="langCode">Language code</param>
        public void SelectLanguage(string langCode)
        {
            LocalizedAlternateText.SelectLanguage(langCode);
            LocalizedDescription.SelectLanguage(langCode);
            LocalizedLinkUrl.SelectLanguage(langCode);
            LocalizedTitle.SelectLanguage(langCode);
        }

        /// <summary>
        /// Fills page model with data
        /// </summary>
        /// <param name="source">Dictionary with values</param>
        /// <param name="args">Should be empty</param>
        public override void Assign(object source, params object[] args)
        {
            base.Assign(source);
            SetCurrentLocalisation();
        }

        public override void Update()
        {
            LocalizedAlternateText.Update();
            LocalizedDescription.Update();
            LocalizedLinkUrl.Update();
            LocalizedTitle.Update();
        }

        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                var _languageSelector = new Flexpage.Models.LanguageSelectorModel(_settings, _flexpageProcessor)
                {
                    CurrentLangCode = this.LocalizedTitle.CurrentLangCode,
                    LangCodes = this.AllFilledKeys,
                    FunctionName = this.ChangeLangFunc,
                };
                return _languageSelector;
            }
        }
    }
}