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
    public class EnumListModel: BlockModel
    {
        public string Uncollapsed { get; set; } = "[]";

        //public LanguageSelectorModel languageSelector = null;
        //public LanguageSelectorModel LanguageSelector
        //{
        //    get
        //    {
        //        if (languageSelector == null)
        //        {
        //            if (Title != null)
        //                languageSelector = new LanguageSelectorModel(_settings) { LangCodes = Title.Localizations.Keys, FunctionName = "fp_webFormChangeLanguage" };
        //        }
        //        return languageSelector;
        //    }
        //}

        public List<EnumModel> Items { get; set; } = new List<EnumModel>();
        public EnumModel CurrentItem { get; set; }

        public EnumListModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            // assign(source, repository.GetFieldTypes());
        }

        public EnumListModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            // assign(source, repository.GetFieldTypes());
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            return null;
        }

        public override void Assign(object source, params object[] args)
        {
            base.Assign(source, args);
            var repo = args[0] as IFlexpageRepository;
            Items = repo.GetEntityList<Enum>().Select(e => new EnumModel(e, Settings, FlexpageProcessor, repo)).ToList();
            CurrentItem = Items.FirstOrDefault();
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title="", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            Assign(null, repository, proto.IsEditor);
        }

        public EnumModel AddItem(IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository)
        {
            var i = new EnumModel(null, settings, flexpage, repository);
            Items.Add(i);
            return i;
        }

        public void Update(IFlexpageRepository repository)
        {
            Update();
        }

    }
}