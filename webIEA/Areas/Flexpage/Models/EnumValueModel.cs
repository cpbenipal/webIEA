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
    public class EnumValueModel: ViewModel
    {
        //private string currentLanguageCode = null;
        //public string CurrentLanguageCode { get => currentLanguageCode;
        //    set
        //    {
        //        currentLanguageCode = value;
        //        displayText = null;
        //    }
        //}
        public int Index { get; set; }
        // public LocalizedStringModel Text { get; set; }
        public string Text { get; set; }
        public string CurrentText { get; set; }
        public string CurrentLanguage { get; set; }
        //public string Text { get; set; }
        //public string displayText = null;
        //public string DisplayText {
        //    get
        //    {
        //        if (displayText == null)
        //        {
        //            var lt = LocalizedStringModel.CreateNew(Text, Settings);
        //            displayText = lt.;
        //        }
        //    }
        //        ; set; }
        public int Value { get; set; }
        public int EnumID { get; set; }
        
        public override void Assign(object source, params object[] args)
        {
            base.Assign(source, args);
            if (source is EnumValue)
            {
                var ev = source as EnumValue;
                this.EnumID = ev.EnumID;
                this.ID = ev.ID;
                this.Index = ev.Order;
                this.Text = ev.Text;
                // var ls = LocalizedStringModel.CreateNew(ev.Text, args[0] as IFlexpageSettings);
                // this.CurrentText = ls.Current;
                CurrentLanguage = _settings.DefaultLangCode;
                UpdateCurrentText();
                this.Value = ev.Value;
            }
            if (source is Pluritech.Pluriworks.Service.DTO.EnumValueModel)
            {
                var ev = source as Pluritech.Pluriworks.Service.DTO.EnumValueModel;
                this.EnumID = ev.EnumID;
                this.ID = ev.ID;
                //this.Index = ev.Order;
                this.Text = ev.Text;
                // var ls = LocalizedStringModel.CreateNew(ev.Text, args[0] as IFlexpageSettings);
                // this.CurrentText = ls.Current;
                CurrentLanguage = _settings.DefaultLangCode;
                UpdateCurrentText();
                this.Value = ev.Value;
            }
        }

        public EnumValueModel() : base(null, null)
        {

        }

        public EnumValueModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {

        }

        public EnumValueModel(EnumValue source, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository, string langCode) 
            : base(settings, flexpage)
        {
            Assign(source, settings, repository);
            SetCurrentLanguage(langCode);
        }
        public EnumValueModel(Pluritech.Pluriworks.Service.DTO.EnumValueModel source, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository, string langCode) 
            : base(settings, flexpage)
        {
            Assign(source, settings, repository);
            SetCurrentLanguage(langCode);
        }
        public object Apply(EnumValue ev)
        {
            ev.ID = ID;
            ev.EnumID = EnumID;
            ev.Order = Index;
            var ls = LocalizedStringModel.CreateNew(ev.Text, _settings, _flexpageProcessor);
            ls.SelectLanguage(CurrentLanguage);
            ls.Current = CurrentText;
            ev.Text = ls.ToJson();
            // ev.Text =  Text;
            ev.Value = Value;
            return null;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            var ev = repository.GetByID<EnumValue>(ID);
            if (ev == null)
            {
                var e = repository.GetByID<Flexpage.Domain.Entities.Enum>(EnumID);
                ev = repository.CreateNewEnumValue(e, null, Value);
            }
            Apply(ev);
            return base.Apply(repository, args);
        }

        public void UpdateText()
        {
            var ls = LocalizedStringModel.CreateNew(Text, _settings, _flexpageProcessor);
            ls.SelectLanguage(CurrentLanguage);
            ls.Current = CurrentText;
            Text = ls.ToJson();
        }

        public void UpdateCurrentText()
        {
            var ls = LocalizedStringModel.CreateNew(Text, _settings, _flexpageProcessor);
            ls.SelectLanguage(CurrentLanguage);
            CurrentText = ls.Current;
        }

        public void Update(IFlexpageSettings settings)
        {
            Settings = settings;
            UpdateText();
        }

        public void SetCurrentLanguage(string langCode)
        {
            CurrentLanguage = langCode;
            UpdateCurrentText();
        }

    }
}