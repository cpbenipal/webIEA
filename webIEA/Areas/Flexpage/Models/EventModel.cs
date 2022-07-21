using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Newtonsoft.Json;
using Flexpage.Code.CMS;
using Flexpage.Helpers;
using System.IO;
using System.Web.Mvc;

namespace Flexpage.Models
{
    [Obsolete("The class has been deprecated.")]
    public class EventModel : ViewModel
    {
        //DB properties
        public int? CategoryID { get; set; }
        public EventCategory EventCategory { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsVisible { get; set; }
        public int ShortViewCmsTextID { get; set; }
        public int FullViewCmsTextID { get; set; }

        //to show on the page
        public string ViewText { get; set; }

        //service
        public string CategoryName { get; set; }
        public bool IsHeaderLink { get; set; }
        public string FullNewsPageUrl { get; set; }
        public List<SelectListItem> AllowedTags { get; set; }
        public LocalizedStringModel LocalizedTitle { get; set; }
        public LocalizedStringModel LocalizedKeywords { get; set; }
        public LocalizedTextModel ShortText { get; set; }
        public LocalizedTextModel FullText { get; set; }

        public List<string> KeywordsList { get; set; }

        public char KeywordsSeparator { get; set; }

        public EventModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            LocalizedTitle = new LocalizedStringModel(settings, flexpage);
            LocalizedKeywords = new LocalizedStringModel(settings, flexpage);
            ShortText = new LocalizedTextModel(settings, flexpage);
            ShortText.ParentModelFieldName = "ShortText";
            FullText = new LocalizedTextModel(settings, flexpage);
            FullText.ParentModelFieldName = "FullText";
            KeywordsList = new List<string>();
            KeywordsSeparator = ',';
        }

        /// <summary>
        /// Fills model content from provided source (Event entity)
        /// </summary>
        /// <param name="source">Provided source (Event entity)</param>
        /// <param name="args"></param>
        public override void Assign(object source, params object[] args)
        {
            base.Assign(source);
            Event eventEntry = source as Event;

            ID = eventEntry.ID;
            CategoryID = eventEntry.CategoryID;
            IsPrimary = eventEntry.IsPrimary;
            EventCategory =eventEntry.EventCategory;
            IsVisible = eventEntry.IsVisible;

            StartDate = eventEntry.StartDate;
            EndDate = eventEntry.EndDate;
            CreatedDate = eventEntry.CreatedDate;

            ImageUrl = eventEntry.ImgUrl;
            ShortViewCmsTextID = eventEntry.ShortViewCmsTextID ?? 0;
            FullViewCmsTextID = eventEntry.FullViewCmsTextID ?? 0;

            //for editing
            LocalizedTitle.Assign(LocalizedField.UnboxValue(eventEntry.Title, true), "Header");
            Title = LocalizedTitle.NotEmptyLocalization;

            //for editing
            LocalizedKeywords.Assign(LocalizedField.UnboxValue(eventEntry.Keywords, true));
            Keywords = LocalizedKeywords.NotEmptyLocalization;

            string[] words = !string.IsNullOrWhiteSpace(Keywords) ? Keywords.Split(KeywordsSeparator) : null;

            KeywordsList = words != null ? words.ToList() : new List<string>();
            if(eventEntry.EventCategory != null)
                CategoryName = LocalizedField.DisplayTextNonEmpty(eventEntry.EventCategory.Name, _settings.GetCurrentOrDefaultLangCode());
        }

        //used to show on the page
        public void LoadCmsText(IFlexpageRepository repository, bool isFullView, string langCode)
        {
            CmsTextModel cms = new CmsTextModel(_settings, _flexpageProcessor);
            cms.Load(repository, new BlockCommandModel(_settings)
            {
                ID = isFullView ? FullViewCmsTextID : ShortViewCmsTextID,
                Parameters = JsonConvert.SerializeObject(new { LangCode = langCode })
            });
            ViewText = cms.FullText.NotEmptyText;
        }

        /// <summary>
        /// Loads object from repository
        /// </summary>
        /// <param name="repository">Repository to load from</param>
        /// <param name="proto">Popup content data</param>
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            Event entry = null;
            if (proto.BlockAlias != repository.CreateNewAlias)
            {
                entry = repository.GetByID<Event>(proto.ID);
            }
            else
            {
                entry = new Event()
                {
                    StartDate = DateTime.Now.Date,
                    Title = "-- Just added record --"
                };
            }

            Assign(entry);

            //for editing only
            AllowedTags = repository.GetEntityList<EventCategory>()
                .Select(r => new SelectListItem() { Text = LocalizedField.DisplayTextNonEmpty(r.Name, _settings.GetCurrentOrDefaultLangCode()), Value = r.ID.ToString() })
                .OrderBy(t => t.Text).ToList();

            //load all cms localizations
            LocalizedTitle.CurrentLangCode = _settings.GetCurrentOrDefaultLangCode();
            LocalizedKeywords.CurrentLangCode = _settings.GetCurrentOrDefaultLangCode();

            ShortText.Load(repository, new BlockCommandModel(_settings) { ID = ShortViewCmsTextID });
            FullText.Load(repository, new BlockCommandModel(_settings) { ID = FullViewCmsTextID });
        }

        /// <summary>
        /// Apply changes to the DB entity
        /// </summary>
        /// <param name="repository"></param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository);

            Event entry = repository.GetByID<Event>(ID);
            if (entry == null)
            {
                entry = repository.CreateNewEvent("admin", false);
            }

            entry.StartDate = this.StartDate;
            entry.EndDate = this.EndDate;
            entry.ImgUrl = this.ImageUrl;
            entry.Title = LocalizedField.BoxValues(this.LocalizedTitle.Localizations);

            var localizations = new Dictionary<string, string>();

            foreach (var localization in this.LocalizedKeywords.Localizations)
            {
                if (localization.Value != null)
                {
                    var keys = localization.Value.Split(new char[] { ';', ',' }, StringSplitOptions.None);
                    localizations.Add(localization.Key, string.Join(",", keys.Select(s => s.Trim()).ToList()));
                }
            }

            entry.Keywords = LocalizedField.BoxValues(localizations);
            entry.IsVisible = IsVisible;
            //block.KeywordsSeparator = this.KeywordsSeparator.ToString();

            //save localizations
            //entry.ShortViewCmsText = SaveCmsLocalization(ShortText, repository, ShortViewCmsTextID);
            //entry.FullViewCmsText = SaveCmsLocalization(FullText, repository, FullViewCmsTextID);
            entry.ShortViewCmsText = ShortText.Apply(repository);
            entry.FullViewCmsText = FullText.Apply(repository);
            
            //store categoryID
            if (this.CategoryID != null)
            {
                entry.CategoryID = this.CategoryID;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(this.CategoryName))
                {
                    //create new category
                    EventCategory newCat = repository.CreateNewEventCategory(LocalizedField.CreateLocalizedString(this.CategoryName, this.CategoryName, this.CategoryName));
                    entry.EventCategory = newCat;
                    entry.CategoryID = newCat.ID;
                }
                else
                {
                    entry.CategoryID = null;
                }
            }

            repository.ApplyChanges();

            return entry;
        }

        private CmsText SaveCmsLocalization(LocalizedTextModel model, IFlexpageRepository repository, int ID)
        {
            CmsText cmstext = repository.GetByBlockID<CmsText>(ID);
            if (cmstext == null)
            {
                cmstext = repository.CreateNewCmsText();
            }

            foreach (string lang in model.Texts.Keys)
            {
                if (string.IsNullOrWhiteSpace(lang))
                    continue;

                Language language = repository.Languages.FirstOrDefault(l => l.Code == lang);
                CmsTextLocalization loc = cmstext.CmsTextLocalizations.FirstOrDefault(l => l.Language.Code == lang);
                if (loc == null)
                {
                    loc = repository.CreateNewLocalization(cmstext, language.ID);
                }
                loc.FullText = model.Texts[lang]?.Text;
            }
            return cmstext;
        }

        /// <summary>
        /// Delete object from repository
        /// </summary>
        /// <param name="repository">Repository to load from</param>
        public override void Delete(IFlexpageRepository repository)
        {
            repository.DeleteEntity<Event>(ID);
            repository.ApplyChanges();
        }


        /// <summary>
        /// Updates localization data
        /// </summary>
        public override void Update()
        {
            LocalizedTitle.Update();
            LocalizedKeywords.Update();
            ShortText.Update();
            FullText.Update();
        }

        public void SelectLocalization(string lang)
        {
            LocalizedTitle.SelectLanguage(lang);
            LocalizedKeywords.SelectLanguage(lang);
            ShortText.SelectLanguage(lang);
            FullText.SelectLanguage(lang);
        }

        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                return GetLanguageSelector(this.ShortText.CurrentLanguage,
                    new List<LocalizedStringModel>() { },
                    new List<LocalizedTextModel>() { this.ShortText, this.FullText });
            }
        }
    }
}