using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Enum = Flexpage.Domain.Entities.Enum;


namespace Flexpage.Models
{

    public class WebFormCAPTCHAFieldValueModel : WebFormFieldValueModel
    {
        public override string View { get => "_WebFormCAPTCHAField"; }
        public string Value { get; set; }

        public WebFormCAPTCHAFieldValueModel() : base(null, null)
        {
        }

        public WebFormCAPTCHAFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {

        }

        public WebFormCAPTCHAFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(field);
        }

    }

}