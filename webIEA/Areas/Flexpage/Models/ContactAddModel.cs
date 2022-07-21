using Flexpage.Abstract;
using Flexpage.Code.StructureManagement;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Flexpage.Helpers;
using Flexpage.Helpers.StructureManagement;
using Pluritech.Contact.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Flexpage.Models
{
    public class ContactAddModel : ViewModel
    {
        public string Title { get; set; }
        public eContactType ContactType { get; set; }
        public ContactDetailsModel ContactDetailsModel { get; set; }

        public ContactAddModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Settings = settings;
        }
        
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            var alias = "_contactSelectorV2_"+ title.Replace('\\',' ');
            this.Title = title;
            if (Title.Contains("Person"))
            {
                ContactType = eContactType.Person;
            }
            else
            if (Title.Contains("Company"))
            {
                ContactType = eContactType.Company;
            }
            ContactDetailsModel = new ContactDetailsModel(Settings, FlexpageProcessor);
            
            ContactDetailsModel.Load(repository, new BlockCommandModel(Settings) { ID = ID }, false,null, ContactType,true,0);
            ContactDetailsModel.AllowEdit = true;
            ContactDetailsModel.Edit = true;
        }
        
    }
}