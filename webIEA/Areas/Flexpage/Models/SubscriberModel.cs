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

namespace Flexpage.Models
{
    public class SubscriberModel: ViewModel
    {
        public int SubscriptionID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Seats { get; set; }
        public string Comment { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsNotified { get; set; }
        public string Language { get; set; }
        public bool Void { get; set; }
        public DateTime? ConfirmationDate { get; set; }

        public virtual List<SubscriberField> Fields { get; set; } = new List<SubscriberField>();

        public override void Assign(object source, params object[] args)
        {
            base.Assign(source, args);
            if (source is Subscriber)
            {
                var subscriber = source as Subscriber;

                ID = subscriber.ID;
                IsConfirmed = subscriber.IsConfirmed;
                IsNotified = subscriber.IsNotified;
                Language = _settings.GetCurrentOrDefaultLangCode();
                Name = subscriber.Name;
                Seats = subscriber.Seats;
                Comment = subscriber.Comment;
                Void = subscriber.Void;
                SubscriptionID = subscriber.SubscriptionID;
                Email = subscriber.Email;
                ConfirmationDate = subscriber.ConfirmationDate;
                Fields = subscriber.SubscriberField.ToList();
            }
        }

        public SubscriberModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {

        }

        public SubscriberModel(Subscriber source, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository, string langCode) 
            : base(settings, flexpage)
        {
            Assign(source, settings, repository);
        }

        public object Apply(Subscriber subscriber)
        {
            subscriber.IsConfirmed = IsConfirmed;
            subscriber.IsNotified = IsNotified;
            subscriber.Language = _settings.GetCurrentOrDefaultLangCode();
            subscriber.Name = Name;
            subscriber.Seats = Seats;
            subscriber.Comment = Comment;
            subscriber.Void = Void;
            subscriber.Email = Email;
            subscriber.ConfirmationDate = ConfirmationDate;

            foreach (var field in subscriber.SubscriberField)
            {
                var subscriberField = Fields.FirstOrDefault(f => f.ID == field.ID);

                if (subscriberField != null)
                    field.Value = subscriberField.Value;
            }

            return null;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            var scrb = repository.GetByID<Subscriber>(ID);

            if (scrb == null)
            {
                var scrp = repository.GetByID<Subscription>(SubscriptionID);
                scrb = repository.CreateNewSubscriber(scrp);
            }
            Apply(scrb);
            repository.ApplyChanges();
            return base.Apply(repository, args);
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);

            var subscriber = repository.GetByID<Subscriber>(proto.ID);

            if (subscriber != null)
                this.Assign(subscriber);
        }
    }
}