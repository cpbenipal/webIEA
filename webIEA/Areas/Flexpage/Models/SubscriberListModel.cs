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
    public class SubscriberListModel : BlockModel
    {
        // public List<SubscriptionModel> Items { get; set; } = new List<SubscriptionModel>();
        public string SubscriptionDescription { get; set; }
        public int SubscriptionID { get; set; }

        public SubscriberListModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
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
            var s = repository.GetByID<Subscription>(proto.ID);
            if (s is Subscription)
            {
                SubscriptionID = proto.ID;
            }
            Assign(null, repository, proto.IsEditor);
        }

        //public SubscriptionModel AddItem(IFlexpageSettings settings, IFlexpageRepository repository)
        //{
        //    var i = new SubscriptionModel(null, settings, repository, CurrentLanguage);
        //    Items.Add(i);
        //    return i;
        //}

        public void Update(IFlexpageRepository repository)
        {
            Update();
        }

    }
}