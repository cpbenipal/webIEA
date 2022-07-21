using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Flexpage.Helpers;
using FlexPage.Helpers;
using System.Configuration;
using System.Text;
using Newtonsoft.Json;
using Enum = Flexpage.Domain.Entities.Enum; 

namespace Flexpage.Models
{
    public class EnumModel: ViewModel
    {
        public List<string> LanguageCodes { get; set; } = new List<string>();
        //public LanguageSelectorModel languageSelector = null;

        public LanguageSelectorModel LanguageSelector { get; set; }

        public string CurrentLanguage { get; set; }

        public string Name { get; set; }

        public List<EnumValueModel> Items { get; set; } = new List<EnumValueModel>();

        public EnumValueModel CurrentItem { get; set; } = null;

        public override void Assign(object source, params object[] args)
        {
            CurrentLanguage = _settings.DefaultLangCode;
            base.Assign(source, args);
            if (source is Enum)
            {
                var e = source as Enum;
                this.ID = e.ID;
                this.Name = e.Name;
            }
        }

        public void LoadContent(Enum source, IFlexpageRepository repository)
        {
            Items = source.EnumValue.Select(e => new EnumValueModel(e, Settings, FlexpageProcessor, repository, Settings.DefaultLangCode)).ToList();
            LanguageSelector = new LanguageSelectorModel(_settings, _flexpageProcessor)
            {
                FunctionName = "fp_enumChangeLanguage",
                isLocalizedStringUsed = false,
                LangCodes=new List<string>()
            };
            LanguageSelector.CurrentLangCode = CurrentLanguage;
            LanguageSelector.Update(CurrentLanguage, GetEnumModelLangCodes(this));
            if (LanguageSelector.LangCodes.Count == 0)
            {
                LanguageSelector.LangCodes.Add(CurrentLanguage);
            }
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title="", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            var source = repository.GetByID<Enum>(proto.ID);
            if (source != null)
            {
                Assign(source);
                LoadContent(source, repository);
                Items.Clear();
            }
        }

        public EnumModel(Enum source, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(source, settings, repository);
        }

        public EnumModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            var source = repository.GetByID<Enum>(ID);
            if (source == null)
            {
                source = repository.CreateNewEnum(Name);
            }
            source.Name = Name;
            return null;
        }

        public EnumValueModel AddItem(IFlexpageRepository repository)
        {
            return null;
        }

        public void Update(IFlexpageRepository repository)
        {
            Update();
            LanguageSelector.CurrentLangCode = CurrentLanguage;
            foreach(var e in Items)
            {
                e.SetCurrentLanguage(CurrentLanguage);
            }
        }

        public void SetCurrentLanguage(string langCode)
        {
            CurrentLanguage = langCode;
            LanguageSelector.CurrentLangCode = CurrentLanguage;
            foreach (var e in Items)
            {
                e.SetCurrentLanguage(CurrentLanguage);
            }
            if (LanguageSelector.LangCodes == null)
            {
                LanguageSelector.LangCodes = new List<string>();
            }
            LanguageSelector.LangCodes.Add(langCode);
        }

        private List<string> GetEnumModelLangCodes(EnumModel model)
        {
            HashSet<string> langcodes = new HashSet<string>();          
            model.Items.ForEach(ev =>
            {
                var ls = LocalizedStringModel.CreateNew(ev.Text, _settings, _flexpageProcessor);
                langcodes.UnionWith(ls.Localizations.Keys);
            });
            return langcodes.ToList();
        }
    }
}