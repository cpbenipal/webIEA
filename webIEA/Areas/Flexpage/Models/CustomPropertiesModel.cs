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
using Pluritech.Pluriworks.Service.DTO;

namespace Flexpage.Models
{
    
    public class CustomPropertiesModel : ViewModel
    {
        public List<Pluritech.Properties.Abstract.DTO.Property> ObjectProperties { get; set; }
        public List<string> Types { get; set; }
        public List<ObjectTypeEntity> ObjectTypes { get; set; }
        public List<string> EditFields { get; set; }
        public List<Pluritech.Pluriworks.Service.DTO.EnumModel> Enums { get; set; }

        public CustomPropertiesModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, bool isNew = false) : base(settings, flexpage)
        {
            EditFields = new List<string>() { "Name"};
            if (isNew)
            {
                EditFields.Add("Type");
            }
            EditFields.Add("EnumID");
            Types = System.Enum.GetValues(typeof(ObjectPropertyType)).Cast<ObjectPropertyType>().Select(v=>v.ToString()).ToList();
        }

        public void Load(IPropertyProvider propertyProvider, List<ObjectTypeEntity> objectTypes, List<Pluritech.Pluriworks.Service.DTO.EnumModel> enums, bool needToLoadContent = true)
        {
            Enums = enums;
            ObjectProperties = propertyProvider.EnumerateAllProperties().OrderBy(p => p.IsSystem).Select(cp => new Pluritech.Properties.Abstract.DTO.Property()
            {
                ID = cp.ID,
                Name = cp.Caption,
                Type = cp.Type,
                ObjectTypes = propertyProvider.GetObjectPropertiesTypes(cp.ID).ToList(),
                IsSystem = cp.IsSystem,
                EnumID = cp.EnumID
            }).ToList();
            ObjectTypes = objectTypes;
        }
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title, bool needToLoadContent = true)
        {
            
        }
    }
}