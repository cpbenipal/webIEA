using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Flexpage.Code.CMS;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Abstract.DTO;
using Language = Flexpage.Domain.Entities.Language;

namespace Flexpage.Models
{
    public class LocalizedTextModel : ViewModel, ILocalizedTextModel
    {
        public string ParentModelFieldName { get; set; }
        public Dictionary<string, LocalizedTextsModel> Texts { get; set; }
        public string CurrentLanguage { get; set; }
        [AllowHtml]
        public string CurrentText { get; set; }
        public Unit Height { get; set; } = new Unit(400, UnitType.Pixel);
        public Unit Width { get; set; } = new Unit(100, UnitType.Percentage);

        public int MaxFileSize { get; set; }

        // used in Subscription Manager (_LocalizedText.cshtml. _HtmlEditor.cshtml)
        public Abstract.DTO.HtmlEditorModel htmlEditor = null;

        public Abstract.DTO.HtmlEditorModel HtmlEditor
        {
            get
            {
                if (htmlEditor == null)
                {
                    htmlEditor = new Abstract.DTO.HtmlEditorModel { Text = CurrentText ?? "", Name = "fp_HtmlEditor_" + ParentModelFieldName, Height = Height, Width = Width };
                }
                return htmlEditor;
            }

            // set => htmlEditor = value;
        }

        public string NotEmptyText
        {
            get
            {
                string retVal = CurrentText;
                //try to find first not empty string
                if (string.IsNullOrEmpty(retVal))
                {
                    if (Texts.ContainsKey(_settings.DefaultLangCode) && !string.IsNullOrEmpty(Texts[_settings.DefaultLangCode].Text))
                    {
                        retVal = Texts[_settings.DefaultLangCode]?.Text;
                    }
                    else
                    {
                        foreach (string key in Texts.Keys)
                        {
                            if (!string.IsNullOrEmpty(Texts[key]?.Text))
                            {
                                retVal = Texts[key]?.Text;
                                break;
                            }
                        }
                    }
                }

                if(_settings.ScrambleEmails)
                {
                    retVal = Helpers.HtmlScrambler.ScrambleEmails(retVal);
                }

                return retVal;
            }
        }

        public LocalizedTextModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) :base(settings, flexpage)
        {
            Texts = new Dictionary<string, LocalizedTextsModel>();
            CurrentLanguage = _settings.GetCurrentOrDefaultLangCode();
            MaxFileSize = _settings.HtmlEditorMaxFileSize;
        }

        /// <summary>
        /// Fills page model with data
        /// </summary>
        /// <param name="source">Dictionary with values</param>
        /// <param name="args">0 - localized field name in the parent class</param>
        public override void Assign(object source, params object[] args)
        {
            base.Assign(source);
            ParentModelFieldName = args[0].ToString();
            Texts = source as Dictionary<string, LocalizedTextsModel>;
            SetCurrentLocalization();
        }

        public void AddText(string lang, string text)
        {
            Texts[lang] = new LocalizedTextsModel(text);
            SetCurrentLocalization();
        }

        public string GetText(string lang)
        {
            if(Texts.ContainsKey(lang))
                return Texts[lang]?.Text;
            return null;
        }

        public string GetText()
        {
            return string.IsNullOrEmpty(CurrentText)
                ? Texts.FirstOrDefault(p => !string.IsNullOrEmpty(p.Value?.Text)).Value?.Text
                : CurrentText;
        }

        public override void Update()
        {
            if (htmlEditor != null)
                CurrentText = htmlEditor.Text;
            Texts[CurrentLanguage] = new LocalizedTextsModel(CurrentText);
        }

        protected void SetCurrentLocalization()
        {
            if (!string.IsNullOrEmpty(CurrentLanguage))
            {
                CurrentText = Texts.ContainsKey(CurrentLanguage) ? Texts[CurrentLanguage]?.Text : "";
                if (htmlEditor != null)
                    htmlEditor.Text = CurrentText;
            }
        }

        public void SelectLanguage(string langCode)
        {
            CurrentLanguage = langCode;
            SetCurrentLocalization();
        }

        public void SelectLanguageAndSetValue(string langCode)
        {
            if(!Texts.ContainsKey(langCode))
                AddText(langCode, Texts[_settings.GetCurrentOrDefaultLangCode()]?.Text);

            CurrentLanguage = langCode;
            SetCurrentLocalization();
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            CurrentLanguage = _settings.GetCurrentOrDefaultLangCode();

            ID = proto.ID;

            CmsText cmsTexts = repository.GetByBlockID<CmsText>(proto.ID);
            if (cmsTexts != null)
            {
                foreach (CmsTextLocalization loc in cmsTexts.CmsTextLocalizations)
                    AddText(loc.Language.Code, loc.FullText);
            }

            base.Load(repository, proto,title, needToLoadContent);
        }

        /// <summary>
        /// Applying the model daya to the entity (with creating the new one if needed) without saving to DB
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public CmsText Apply(IFlexpageRepository repository)
        {
            CmsText cmstext = repository.GetByBlockID<CmsText>(ID);
            if (cmstext == null)
            {
                cmstext = repository.CreateNewCmsText();
            }

            foreach (string lang in Texts.Keys)
            {
                if (string.IsNullOrWhiteSpace(lang))
                    continue;

                Language language = repository.Languages.FirstOrDefault(l => l.Code == lang);
                CmsTextLocalization loc = cmstext.CmsTextLocalizations.FirstOrDefault(l => l.Language.Code == lang);
                if (loc == null)
                {
                    loc = repository.CreateNewLocalization(cmstext, language.ID);
                }
                loc.FullText = Texts[lang]?.Text;
            }

            return cmstext;
        }

        public LocalizedStringModel ToLocalizedStringModel(bool stripHTML, int maxLength)
        {
            var r = new LocalizedStringModel();
            r.Localizations = new Dictionary<string, string>();
            if (stripHTML)
            {
                foreach (var p in Texts)
                {
                    var s = Helpers.StringHelper.RemoveTagsAndEscapeChars(Helpers.StringHelper.RemoveTags(p.Value?.Text));
                    r.Localizations.Add(p.Key, s);
                }
                if (maxLength > 0)
                {
                    r = r.Truncate(maxLength);
                }

            }
            else
            {
                foreach (var p in Texts)
                {
                    r.Localizations.Add(p.Key, p.Value?.Text);
                }
            }
               
            return r;
        }
    }
}