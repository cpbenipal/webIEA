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

    public class WebFormIntFieldValueModel : WebFormFieldValueModel
    {
        public override string View { get => "_WebFormIntField"; }
        public int Value { get; set; }

        public WebFormIntFieldValueModel() : base(null, null)
        {
        }

        public WebFormIntFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public WebFormIntFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(field);
        }

        public override void Validate(ModelStateDictionary modelState, string key)
        {
            base.Validate(modelState, key);
            int v;
            if (!int.TryParse(this.StringValue, out v))
                modelState.AddModelError(key, "The type of this field should be integer");
        }

    }

}