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

    public class WebFormImageFieldValueModel : WebFormFieldValueModel
    {
        public override string View { get => "_WebFormImageField"; }
        public string Value { get; set; }

        public WebFormImageFieldValueModel() : base(null, null)
        {
        }

        public WebFormImageFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public WebFormImageFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(field);
        }

    }

}