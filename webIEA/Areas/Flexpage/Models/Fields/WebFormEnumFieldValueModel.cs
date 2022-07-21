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

    public class WebFormEnumFieldValueModel : WebFormFieldValueModel
    {
        public override string View { get => "_WebFormEnumField"; }
        public int EnumID { get; set; }
        public int Value { get; set; }

        public WebFormEnumFieldValueModel() : base(null, null)
        {
        }

        public WebFormEnumFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public WebFormEnumFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(field, repository);
        }

        public override void Setup(FormField source, IFlexpageRepository repository)
        {
            base.Setup(source, repository);
            Items = source.Enum.EnumValue.ToList();
        }
    }
}