using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Enum = Flexpage.Domain.Entities.Enum;


namespace Flexpage.Models
{

    public class WebFormEmailFieldValueModel : WebFormFieldValueModel
    {
        public override string View { get => "_WebFormStringField"; }
        public string Value { get; set; }

        public WebFormEmailFieldValueModel() : base(null, null)
        {
        }

        public WebFormEmailFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public WebFormEmailFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(field);
        }
    }

}