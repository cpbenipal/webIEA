using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
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
using Flexpage.Domain.Entities;

namespace Flexpage.Models
{
    public class SubscriptionTextModel : ViewModel
    {
        public Dictionary<int, string> NameMapping { get; set; }

        public string DisplayName { get; set; }
        public int NotificationTypeId { get; set; }
        public int TypeId { get; set; }
        public int? SubscriptionID { get; set; }
        // public Subscription Subscription { get; set; }
        public int? CmsTextID { get; set; }
        public int Index { get; set; }
        public virtual CmsTextModel CmsText { get; set; }

        private int _idNewEmail = 2;
        private int _idDescription = 1;

        public override void Assign(object source, params object[] args)
        {
            base.Assign(source, args);
            if (source is SubscriptionText)
            {
                var n = source as SubscriptionText;
                ID = n.ID;
                SubscriptionID = n.SubscriptionID;
                CmsTextID = n.CmsTextID;
                NotificationTypeId = n.NotificationTypeID?? _idNewEmail;
                DisplayName = n.NotificationType?.Name;
                CmsText = new CmsTextModel(n.CmsText, Settings, FlexpageProcessor);
            }
            else
            {
                ID = -1;
                SubscriptionID = -1;
                CmsTextID = -1;
                NotificationTypeId = _idNewEmail;
                DisplayName = "New email";
                CmsText = new CmsTextModel(Settings, FlexpageProcessor);
            }
        }

        public void LoadContent(Subscription source, IFlexpageSettings settings, IFlexpageRepository repository)
        {
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            var source = repository.GetByID<Enum>(proto.ID);
            if (source != null)
            {
                Assign(source, _settings);

                // LoadContent(source, _settings, repository);
            }

            SetNameMapping(repository);
        }

        public SubscriptionTextModel(SubscriptionText source, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository) 
            : base(settings, flexpage)
        {
            Assign(source, settings);
            SetNameMapping(repository);
        }
        public SubscriptionTextModel(SubscriptionText source, IFlexpageSettings settings, Abstract.IFlexpage flexpage)
            : base(settings, flexpage)
        {
            Assign(source, settings);
        }
        public void SetNameMapping(IFlexpageRepository repository)
        {
            NameMapping = repository
                .GetEntityList<SubscriptionNotificationType>()
                .Where(w => w.ID != _idDescription)
                .ToDictionary(key => key.ID, value => value.Name);
        }

        public SubscriptionTextModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Assign(settings);
        }

        public void Apply(SubscriptionText target, IFlexpageRepository repository)
        {
            target.NotificationTypeID = NotificationTypeId;
            CmsText.Alias = null;
            CmsText.Apply(repository, target.CmsText);
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            var source = args[0] as SubscriptionText;
            //var source = repository.GetByID<SubscriptionText>(ID);
            //if (source == null)
            //{
            //    source = repository.CreateNewSubscriptionText(args[0] as Subscription);
            //}
            Apply(source, repository);
            return null;
        }

        public void Update(IFlexpageRepository repository)
        {
        }

        public override void Update()
        {
            base.Update();
            CmsText.Update();
        }

        public void SetCurrentLanguage(string langCode)
        {
            CmsText.SelectLanguage(langCode);
        }

    }
}