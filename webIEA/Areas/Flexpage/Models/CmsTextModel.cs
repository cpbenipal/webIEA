using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Flexpage.Code.CMS;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;

namespace Flexpage.Models
{
    public class CmsTextModel : BlockModel
    {
        public bool IncludeInSearch { get; set; }

        public LocalizedStringModel LocalizedTitle { get; set; }
        public LocalizedTextModel FullText { get; set; }

        public override void Assign(object source, params object[] args)
        {
            if(source is CmsText)
            {
                CmsText cmsText = source as CmsText;

                if(cmsText.Block != null)
                    base.Assign(cmsText.Block, args);

                assign(cmsText);
            }
        }

        protected void assign(CmsText source)
        {
            this.ID = source.BlockID;
            this.IncludeInSearch = source.IncludeInSearch;
            
            LocalizedTitle.CurrentLangCode = _settings.GetCurrentOrDefaultLangCode();
            FullText.CurrentLanguage = _settings.GetCurrentOrDefaultLangCode();
            foreach(CmsTextLocalization loc in source.CmsTextLocalizations)
            {
                FullText.AddText(loc.Language.Code, loc.FullText);
                LocalizedTitle.AddText(loc.Language.Code, loc.Title);
            }
           
        }

        public CmsTextModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            //this.BlockType = "CmsText";
            LocalizedTitle = new LocalizedStringModel(settings, flexpage);
            FullText = new LocalizedTextModel(settings, flexpage)
            {
                ParentModelFieldName = "FullText"
            };
        }

        public CmsTextModel(CmsText source, Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            //this.BlockType = "CmsText";
            LocalizedTitle = new LocalizedStringModel(settings, flexpage);
            FullText = new LocalizedTextModel(settings, flexpage)
            {
                ParentModelFieldName = "FullText"
            };
            assign(source);
        }

        public void Apply(IFlexpageRepository repository, CmsText target)
        {
            // target.ID = this.ID;
            target.Block.Alias = this.Alias;
            target.IncludeInSearch = this.IncludeInSearch;
            //target.Block.Visible = Visible;
            //target.Block.CssClass = CssClass;

            foreach (string lang in LocalizedTitle.Localizations.Keys)
            {
                Language language = repository.Languages.FirstOrDefault(l => l.Code == lang);
                if(language != null)
                {
                    CmsTextLocalization loc = target.CmsTextLocalizations.FirstOrDefault(l => l.LanguageID == language.ID);
                    if(loc == null)
                        loc = repository.CreateNewLocalization(target, language.ID);

                    loc.FullText = FullText.GetText(lang);
                    loc.Title = LocalizedTitle.GetText(lang);
                }                
            }
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            CmsText cmsText = repository.GetByBlockID<CmsText>(ID);
            if(cmsText == null || Alias == repository.CreateNewAlias) // ???
            {
                if(Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                cmsText = repository.CreateNewCmsText( Visible, CssClass);
                Apply(repository, cmsText);
                if (BlocklistID > -1)
                    repository.AddBlockToBlockList(cmsText.Block, BlocklistID, BlockAfter);
            }
            else
            {
                Apply(repository, cmsText);
            }
            repository.ApplyChanges();

            return cmsText;
        }

        private void AssignDefaultValues(IFlexpageRepository repository, string predefinedAlias)
        {
            Language l = repository.DefaultLanguage;            
            this.ID = -1;
            this.IncludeInSearch = true;
            this.Alias = string.IsNullOrWhiteSpace(predefinedAlias) ? repository.CreateNewAlias : predefinedAlias;
        }       

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title="", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            this.Alias = proto.BlockAlias;
            ID = proto.ID;
            CmsText item = null;

            if(proto.ID == BlockModel.NewStaticBlockID && !string.IsNullOrEmpty(proto.BlockAlias))
            {
                item = repository.GetByAlias<CmsText>(Alias);
            }
            else
            {
                item = repository.GetByBlockID<CmsText>(ID);
            }

            if (item == null)
            {
                AssignDefaultValues(repository, proto.BlockAlias);                
            }
            else
            {
                Assign(item, proto.BlockAlias);                
            }            
        }

        public override void Update()
        {
            base.Update();
            LocalizedTitle.Update();
            FullText.Update();
        }


        public override void Delete(IFlexpageRepository repository)
        {
            base.Delete(repository);
        }

        public void SelectLanguage(string langCode)
        {
            LocalizedTitle.SelectLanguage(langCode);
            FullText.SelectLanguage(langCode);
            // LanguageSelector.Update(langCode, Title.Localizations.Keys);
        }

        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                return GetLanguageSelector(this.FullText.CurrentLanguage,
                    new List<LocalizedStringModel>() { this.LocalizedTitle },
                    new List<LocalizedTextModel>() {  this.FullText }, 
                    "fp_cmsTextChangeLanguage");
            }
        }
    }
}