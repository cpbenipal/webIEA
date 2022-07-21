using Flexpage.Abstract;
using Flexpage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class LocalizedModel : ViewModel
    {
        public LocalizedModel (IFlexpageSettings settings, IFlexpage flexpageProcessor) : base(settings, flexpageProcessor)
        {
        }
    }
}