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

    public class WebFormDoubleFieldValueModel : WebFormFieldValueModel
    {
        public override string View { get => "_WebFormDoubleField"; }
        public double Value { get; set; }

        public WebFormDoubleFieldValueModel() : base(null, null)
        {
        }

        public WebFormDoubleFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public WebFormDoubleFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(field, repository);
        }

        public override void Validate(ModelStateDictionary modelState, string key)
        {
            base.Validate(modelState, key);
            double v;
            if (!double.TryParse(this.StringValue, out v))
                modelState.AddModelError(key, "The type of this field should be double");
        }

    }

}