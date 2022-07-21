using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;

namespace Flexpage.Models
{
    public class TinyUrlModel : BlockModel
    {
        public string ShortUrl { get; set; }
        public string NavigateUrl { get; set; }
        public int? LanguageID { get; set; }
        public string Language { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsShowDestinationUrl { get; set; }

        private int websiteID { get; set; }

        public TinyUrlModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) :base(settings, flexpage)
        {
            this.websiteID = settings.WebsiteID;
        }

        public TinyUrlModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, TinyUrl source):this(settings, flexpage)
        {
            this.ID = source.ID;
            this.ShortUrl = source.ShortUrl;
            this.NavigateUrl = source.NavigateUrl;
            this.LanguageID = source.LanguageID;
            if (source.Language != null)
                Language = source.Language.Name;
            this.IsDisabled = source.IsDisabled;            
        }

        /// <summary>
        /// Fills model content from provided source (TinyUrl entity)
        /// </summary>
        /// <param name="source">Provided source (TinyUrl entity)</param>
        /// <param name="args"></param>
        public override void Assign(object source, params object[] args)
        {
            base.Assign(source);
            TinyUrl s = source as TinyUrl;

            ID = s.ID;

            this.ShortUrl = s.ShortUrl;
            this.NavigateUrl = s.NavigateUrl;
            this.LanguageID = s.LanguageID;
            this.IsDisabled = s.IsDisabled;
            this.IsShowDestinationUrl = s.IsShowDestinationUrl;

            if (s.Language != null)
                Language = s.Language.Name;

        }

        /// <summary>
        /// Loads object from repository
        /// </summary>
        /// <param name="repository">Repository to load from</param>
        /// <param name="proto">Popup content data</param>
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            TinyUrl block = null;
            int id = proto.ID;
            block = repository.GetByID<TinyUrl>(id);

            if(block == null)
            { 
                block = new TinyUrl();
            }

            Assign(block, id);
        }

        /// <summary>
        /// Apply changes to the DB entity
        /// </summary>
        /// <param name="repository"></param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository);

            TinyUrl block = repository.GetByID<TinyUrl>(ID);
            if (block == null)
            {
                block = repository.CreateNewTinyUrl("", "");
            }

            block.ShortUrl = this.ShortUrl;
            block.NavigateUrl = this.NavigateUrl;
            block.LanguageID = this.LanguageID;
            block.IsDisabled = this.IsDisabled;
            block.IsShowDestinationUrl = this.IsShowDestinationUrl;
            block.WebsiteID = this.websiteID;

            repository.ApplyChanges();

            return null;
        }

        /// <summary>
        /// Delete object from repository
        /// </summary>
        /// <param name="repository">Repository to load from</param>
        public override void Delete(IFlexpageRepository repository)
        {
            repository.DeleteEntity<TinyUrl>(ID);
            repository.ApplyChanges();
        }


        /// <summary>
        /// Updates localization data
        /// </summary>
        public override void Update()
        {
            // ???
        }

        public override void FillViewData(ViewDataDictionary viewData, IFlexpageRepository repository, string Title = "")
        {
            base.FillViewData(viewData, repository);

            viewData["LanguageList"] = repository.GetLanguages()
                .ConvertAll<KeyValuePair<int, string>>(a => new KeyValuePair<int, string>(a.ID, a.Name));
        }
    }
}