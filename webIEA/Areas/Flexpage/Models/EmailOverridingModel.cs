
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Flexpage.Models
{
    public enum TypeEmailOverriding
    {
        UseDefault,Overide, Email
    }
    public class EmailOverridingModel : ViewModel
    {
        public TypeEmailOverriding Type { get; set; }
        public int? DefaultID { get; set; }
        public string Default { get; set; }
        public int? OverideID { get; set; }
        public SelectList Overides { get; set; }
        public string Email { get; set; }
        public eContactType ContactType { get; set; }
        public int ContactID { get; set; }
        public int СontactShortcutID { get; set; }
        public int FolderID { get; set; }
        public bool ApplyToAllContactFolders { get; set; }
        public EmailOverridingModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
        }
        
        public virtual void Load(IFlexpageRepository repository, IContactProvider _contactProvider, BlockCommandModel proto,int contactID,int contactShortcutID, 
            eContactType contactType, int folderID, string title="", bool needToLoadContent = true)
        {
            ContactType = contactType;
            ContactID = contactID;
            СontactShortcutID = contactShortcutID;
            FolderID = folderID;
            DefaultID = _contactProvider.GetDefaultEmailID(ContactID, ContactType);
            OverideID = _contactProvider.GetOverideEmailID(СontactShortcutID, ContactType, folderID);
            Default =  _contactProvider.GetDefaultEmail(ContactID, ContactType);
            Email = OverideID == null || OverideID == 0 ? _contactProvider.GetOverideEmail(СontactShortcutID, ContactType, folderID) : String.Empty;
            Overides = new SelectList(_contactProvider.GetEmail(ContactID, ContactType), "ID", "Value");
            if (!string.IsNullOrEmpty(Email))
            {
                Type = TypeEmailOverriding.Email;
            }
            else if (OverideID != null && OverideID != 0)
            {
                Type = TypeEmailOverriding.Overide;
            }
            base.Load(repository, proto,title, needToLoadContent);
        }
        
    }
}