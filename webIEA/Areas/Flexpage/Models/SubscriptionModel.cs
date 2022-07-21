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
    public class SubscriptionModel: BlockModel
    {
        public List<string> LanguageCodes { get; set; } = new List<string>();

        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                return GetLanguageSelector(this.Description?.CmsText?.FullText?.CurrentLanguage?? Settings.GetCurrentOrDefaultLangCode(),
                    new List<LocalizedStringModel>() { this.ShortDescription },
                    new List<LocalizedTextModel>() { this.Description?.CmsText?.FullText}.Union(this.SubscriptionText.Select(st=>st.CmsText?.FullText)).ToList(),
                    "fp_subscriptionChangeLanguage");
            }
        }


        public bool Enabled { get; set; }
        public string SubscriptionForm { get; set; }
        public int MaxSeats { get; set; }
        public string Code { get; set; }
        public string SubscriptionPage { get; set; }
        public LocalizedStringModel ShortDescription { get; set; }
        public SubscriptionTextModel Description { get; set; } = null;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Void { get; set; }
        public bool SendRegistrationMail { get; set; }
        public string Uncollapsed { get; set; } = "[]";

        public List<SubscriptionTextModel> SubscriptionText { get; set; } = new List<SubscriptionTextModel>();

        private int _idDescription = 1;

        private SubscriptionTextModel GetDescription(IFlexpageRepository repository)
        {
            var subscriptionText = repository.GetEntityList<SubscriptionText>().LastOrDefault(w => w.NotificationTypeID == _idDescription&&w.SubscriptionID== ID);
            var description = new SubscriptionTextModel(subscriptionText ??
                    new Domain.Entities.SubscriptionText()
                    {
                        NotificationTypeID = _idDescription,
                        CmsText = new CmsText(),
                        SubscriptionID=ID,
                        ID=-1
                    },
                    Settings, FlexpageProcessor);
            if (subscriptionText != null)
            {
                description.Apply(subscriptionText, repository);
            }
            return description;
        }
        public override void Assign(object source, params object[] args)
        {
            base.Assign(source);
            if (source is Subscription)
            {
                var s = source as Subscription;
                ID = s.ID;
                Enabled = s.Enabled;
                SubscriptionForm = s.SubscriptionForm;
                MaxSeats = s.MaxSeats;
                Code = s.Code;
                SubscriptionPage = s.SubscriptionPage;

                if (args != null && args.Count() > 0)
                {
                    var repository = args[0] as IFlexpageRepository;
                    Description = GetDescription(repository);
                    ShortDescription = LocalizedStringModel.CreateNew(CmsTextToLocalizedString(Description?.CmsText, 2048, false), Settings, FlexpageProcessor);
                }
                else
                {
                    ShortDescription = LocalizedStringModel.CreateNew(string.Empty, Settings, FlexpageProcessor);
                }
                

                StartDate = s.StartDate;
                EndDate = s.EndDate;
                Void = s.Void;
                SendRegistrationMail = s.SendRegistrationMail;
            }
        }

        public void LoadContent(Subscription source, IFlexpageRepository repository)
        {
            Description = GetDescription(repository);
            Description.CmsText.FullText.ParentModelFieldName = "Description";
            Description.CmsText.FullText.Height = new System.Web.UI.WebControls.Unit(200, System.Web.UI.WebControls.UnitType.Pixel);
            SubscriptionText = source.SubscriptionText.Where(w => w.NotificationTypeID != _idDescription)
                .Select(e => new SubscriptionTextModel(e, Settings, FlexpageProcessor, repository)).ToList();
            int i = 0;
            foreach (var e in SubscriptionText)
            {
                e.Index = i;
                e.CmsText.FullText.ParentModelFieldName = String.Format("SubscriptionText{0}", i);
                e.CmsText.FullText.Height = new System.Web.UI.WebControls.Unit(200, System.Web.UI.WebControls.UnitType.Pixel);
                i++;
            }
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            var source = repository.GetByID<Subscription>(proto.ID);
            Description = GetDescription(repository);
            Description.CmsText.FullText.ParentModelFieldName = "Description";
            Description.CmsText.FullText.Height = new System.Web.UI.WebControls.Unit(200, System.Web.UI.WebControls.UnitType.Pixel);
            if (source != null)
            {
                Assign(source, repository);
                LoadContent(source, repository);
            }
            else
            {
                Code = Guid.NewGuid().ToString();
            }
        }

        public SubscriptionModel(Subscription source, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository, string langCode) 
            : base(settings, flexpage)
        {
            Assign(source,repository);
            if (string.IsNullOrEmpty( langCode ))
            {
                langCode = settings.GetCurrentOrDefaultLangCode();
            }
            SetCurrentLanguage(langCode);
        }

        public SubscriptionModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            
        }

        private string CmsTextToLocalizedString(CmsTextModel source, int maxLength, bool useTitle)
        {
            if (useTitle)
                return source.LocalizedTitle.Truncate(maxLength).ToJson();
            else
                return source.FullText.ToLocalizedStringModel(true, maxLength).ToJson();
        }

        public void Apply(Subscription target, IFlexpageRepository repository)
        {
            target.ID = ID;
            target.Enabled = Enabled;
            target.SubscriptionForm = SubscriptionForm;
            target.MaxSeats = MaxSeats;
            target.Code = Code;
            target.SubscriptionPage = SubscriptionPage;
            target.StartDate = StartDate;
            target.EndDate = EndDate;
            target.Void = Void;
            target.SendRegistrationMail = SendRegistrationMail;
            SubscriptionText = SubscriptionText?? new List<SubscriptionTextModel>();
            var targetSubscriptionText = target?.SubscriptionText?.Where(w => w.NotificationTypeID != _idDescription)?.ToList() ?? new List<SubscriptionText>() ;
            targetSubscriptionText.ForEach(sText =>
            {
                var newST = SubscriptionText.Find(st => st.ID == sText.ID);
                if (newST != null)
                {
                    newST.Apply(sText, repository);
                }
                else
                {
                    repository.RemoveSubscriptionText(target, sText);
                }
            });

            SubscriptionText.Where(st => !targetSubscriptionText.Exists(tst => tst.ID == st.ID)).ToList().ForEach(sText =>
            {
                var newST = repository.CreateNewSubscriptionText(target, sText.NotificationTypeId);
                sText.Apply(newST, repository);
            });
            repository.ApplyChanges();
            var descriptionST = target.SubscriptionText.FirstOrDefault(w => w.NotificationTypeID == _idDescription);
            if (descriptionST == null)
            {
                descriptionST = repository.CreateNewSubscriptionText(target, _idDescription);
            }
            repository.ApplyChanges();
            Description.Apply(descriptionST, repository);
            repository.ApplyChanges();
            //target.ShortDescription = CmsTextToLocalizedString(Description?.CmsText, 2048, false);
            repository.ApplyChanges();
            
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            var source = repository.GetByID<Subscription>(ID);
            if (source == null)
            {
                source = repository.CreateNewSubscription();
            }
            Apply(source, repository);
            return null;
        }

        public void Update(IFlexpageRepository repository)
        {
            if (ShortDescription != null)
                ShortDescription.Update();
            if (Description != null)
                Description.Update();

            if (SubscriptionText != null)
                foreach (var e in SubscriptionText)
                    e.Update();

            SetNameMapping(repository);
        }

        public void SetCurrentLanguage(string langCode)
        {
            if (ShortDescription != null)
                ShortDescription.SelectLanguage(langCode);
            if (Description != null)
                Description.SetCurrentLanguage(langCode);
            
            if (SubscriptionText != null)
                foreach (var e in SubscriptionText)
                    e.SetCurrentLanguage(langCode);
        }

        public bool DeleteText(int index)
        {
            SubscriptionTextModel st = null;
            if (SubscriptionText != null)
                st = SubscriptionText.FirstOrDefault(e => e.Index == index);
            var r = st != null;
            if (r)
            {
                SubscriptionText.Remove(st);
                int i = 0;
                foreach (var e in SubscriptionText)
                {
                    e.Index = i;
                    e.CmsText.FullText.ParentModelFieldName = String.Format("SubscriptionText{0}", e.Index);
                    i++;
                }
            }
            return r;
        }

        public SubscriptionTextModel AddText(IFlexpageRepository repository)
        {
            var r = new SubscriptionTextModel(null,Settings, FlexpageProcessor, repository);
            r.Index = SubscriptionText.Count;
            r.CmsText.FullText.ParentModelFieldName = String.Format("SubscriptionText{0}", r.Index);
            r.CmsText.FullText.Height = new System.Web.UI.WebControls.Unit(200, System.Web.UI.WebControls.UnitType.Pixel);
            SubscriptionText.Add(r);
            //SetNameMapping(repository);

            return r;
        }

        public void SetNameMapping(IFlexpageRepository repository)
        {
            var nameMapping = repository.GetEntityList<SubscriptionNotificationType>().Where(w => w.ID != _idDescription).ToDictionary(key => key.ID, value => value.Name);

            SubscriptionText?.ForEach(item =>
            {
                item.NameMapping = nameMapping;
                item.DisplayName = repository.GetByID<SubscriptionNotificationType>(item.NotificationTypeId).Name;
            });
        }
    }
}