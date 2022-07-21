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

    public class WebFormBoolFieldValueModel : WebFormFieldValueModel
    {
        public override string View { get => "_WebFormBoolField"; }
        public bool Value { get; set; }

        public WebFormBoolFieldValueModel() : base(null, null)
        {
        }

        public WebFormBoolFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public WebFormBoolFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(field, repository);
        }

        public override void Validate(ModelStateDictionary modelState, string key)
        {
            base.Validate(modelState, key);
            bool v;
            if (!bool.TryParse(this.StringValue, out v))
                modelState.AddModelError(key, "The type of this field should be boolean");
        }

    }

}