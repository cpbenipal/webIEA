using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;

namespace Flexpage.Models
{
    public class LanguageModel: ViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public LanguageModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage)
            :base(settings, flexpage)
        {

        }

        public LanguageModel(Language source, Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) 
            :this(settings, flexpage)
        {
            this.ID = source.ID;
            this.Code = source.Code;
            this.Name = source.Name;
        }
    }
}