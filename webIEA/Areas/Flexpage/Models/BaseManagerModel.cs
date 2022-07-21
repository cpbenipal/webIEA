using Flexpage.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flexpage.Domain.Enum;

namespace Flexpage.Models
{
    public class BaseStepModel: BlockModel
    {
        public BaseStepModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) 
            : base(settings, flexpageProcessor)
        {
        }

        public WidgetStep Step { get; set; }
    }

    public class GalleryManagerModel: BaseStepModel
    {
        public GalleryManagerModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) 
            : base(settings, flexpageProcessor)
        {
        }

        public GalleryBlockType GalleryBlockType { get; set; } = GalleryBlockType.NotSet;
    }
}