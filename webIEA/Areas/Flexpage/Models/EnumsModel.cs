using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using Flexpage.Domain.Entities;
using Flexpage.Models;
using Pluritech.Permissions.Abstract.DTO;
using Pluritech.Properties.Abstract;
using Pluritech.Properties.Abstract.DTO;
using System.Collections.Generic;
using System.Linq;
using System;
using ObjectPropertyType = Pluritech.Properties.Abstract.DTO.ObjectPropertyType;

namespace Flexpage.Models
{
    
    public class EnumsModel : ViewModel
    {
        public List<Domain.Entities.Enum> Enums { get; set; }
        public EnumsModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title, bool needToLoadContent = true)
        {
            Enums = repository.GetEntityList<Flexpage.Domain.Entities.Enum>();
        }
    }
}