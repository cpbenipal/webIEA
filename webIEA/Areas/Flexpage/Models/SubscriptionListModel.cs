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
    public class SubscriptionListModel : BlockModel
    {
        public string Uncollapsed { get; set; } = "[]";


        public string CurrentLanguage { get; set; }


        public SubscriptionListModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            SetCurrentLanguage(settings.GetCurrentOrDefaultLangCode());
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
            // Items = repo.GetEntityList<Subscription>().Select(e => new SubscriptionModel(e, Settings, repo, CurrentLanguage)).ToList();
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            Assign(null, repository, proto.IsEditor);
        }

        //public SubscriptionModel AddItem(IFlexpageSettings settings, IFlexpageRepository repository)
        //{
        //    var i = new SubscriptionModel(null, settings, repository, CurrentLanguage);
        //    Items.Add(i);
        //    return i;
        //}

        public void SetCurrentLanguage(string langCode)
        {
            CurrentLanguage = langCode;
        }

        public void Update(IFlexpageRepository repository)
        {
            Update();
        }

    }
}