using Flexpage.Abstract;
using Flexpage.Code.StructureManagement;
using Flexpage.Domain.Abstract;
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

namespace Flexpage.Models
{
    public class ContactsAdvancedSearchModel : ViewModel
    {
        public Dictionary<int, ContactField> PersonContactFields { get; set; }
        public Dictionary<int, ContactField> CompanyContactFields { get; set; }
        public Dictionary<int, ContactField> FolderContactFields { get; set; }
        public List<string> PersonToPersonLinkTypes { get; set; }
        public List<string> PersonToCompanyLinkTypes { get; set; }
        public List<string> PersonToFolderLinkTypes { get; set; }
        public List<string> CompanyToPersonLinkTypes { get; set; }
        public List<string> CompanyToCompanyLinkTypes { get; set; }
        public List<string> CompanyToFolderLinkTypes { get; set; }
        public string Folder { get; set; }
        public string StartXML { get; set; }
        public ContactsAdvancedSearchModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
           
        }
        
        public void Load(IFlexpageRepository repository, string alias, 
            Dictionary<int, ContactField> PersonContactFields, 
            Dictionary<int, ContactField> CompanyContactFields, 
            Dictionary<int, ContactField> FolderContactFields,
            List<string> PersonToPersonLinkTypes,
            List<string> PersonToCompanyLinkTypes,
            List<string> PersonToFolderLinkTypes,
            List<string> CompanyToPersonLinkTypes,
            List<string> CompanyToCompanyLinkTypes,
            List<string> CompanyToFolderLinkTypes,
            string folder,
            string startXML="",
            bool needToLoadContent = true)
        {
            this.PersonContactFields = PersonContactFields;
            this.CompanyContactFields = CompanyContactFields;
            this.FolderContactFields = FolderContactFields;
            this.PersonToPersonLinkTypes = PersonToPersonLinkTypes;
            this.PersonToCompanyLinkTypes = PersonToCompanyLinkTypes;
            this.PersonToFolderLinkTypes = PersonToFolderLinkTypes;
            this.CompanyToPersonLinkTypes = CompanyToPersonLinkTypes;
            this.CompanyToCompanyLinkTypes = CompanyToCompanyLinkTypes;
            this.CompanyToFolderLinkTypes = CompanyToFolderLinkTypes;
            this.Folder = folder;
            this.StartXML = ToJSON(startXML);
        }

        public override void Update()
        {
           
        }
        public void Assign(Maintenance source)
        {
           
        }
    }
}