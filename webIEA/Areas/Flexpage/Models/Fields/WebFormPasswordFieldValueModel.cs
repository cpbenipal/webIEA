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

    public class WebFormPasswordFieldValueModel : WebFormFieldValueModel
    {
        public override string View { get => "_WebFormPasswordField"; }
        public double Value { get; set; }

        public WebFormPasswordFieldValueModel() : base(null, null)
        {
        }

        public WebFormPasswordFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public WebFormPasswordFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
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