using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Flexpage.Models
{
    public class BrowserContactsModel : BlockModel
    {
        public ContactsEnumerationModel ContactsEnumerationModel { get; set; }
        public ContactDetailsModel ContactDetailsModel { get; set; }
        public string ContactDetailsAlias { get; set; }
        public string ContactEnumerationAlias { get; set; }

        public BrowserContactsModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : this(settings, flexpage, null)
        {
        }

        public BrowserContactsModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, NameValueCollection queryString) : base(settings, flexpage)
        {
            this.IsEdit = false;
            ContactsEnumerationModel = new ContactsEnumerationModel(settings, flexpage, queryString);
            ContactDetailsModel = new ContactDetailsModel(settings, flexpage);
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            // there are no settings for this block, if its already created then nothing needs to be changed
            if (!string.IsNullOrEmpty(Alias) && repository.GetByAlias<BrowserContacts>(Alias)?.Block != null)
                return null;
            Block browser = repository.CreateNewBrowserContacts();
            if (BlocklistID > -1)
                repository.AddBlockToBlockList(browser, BlocklistID, BlockAfter);
            repository.ApplyChanges();
            browser.Alias = string.IsNullOrEmpty(Alias) ? $"_contact_browser_{browser.ID}" : Alias;
            repository.ApplyChanges();
            return null;
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            ID = proto.ID;
            Load(repository, proto.BlockAlias, null, eContactType.None, null, null, null, needToLoadContent);
        }

        public void Load(IFlexpageRepository repository, string alias, int? id, eContactType type, int? shortcutID, IContactProvider contactProvider, string xml, bool needToLoadContent = true)
        {
            var source = repository.GetByAlias<BrowserContacts>(alias);
            if (source != null)
            {
                Assign(source);
            }
            else
            {
                Alias = alias;
            }
            alias = "_browsercontacts_" + alias;
            
            if (String.IsNullOrEmpty(ContactEnumerationAlias))
            {
                ContactEnumerationAlias = "_ContactsEnumeration" + alias;
            }
            if (String.IsNullOrEmpty(ContactDetailsAlias))
            {
                ContactDetailsAlias = "_ContactDetails" + alias;
            }

            var blockContactsEnumeration = repository.GetByAlias<ContactEnumeration>(ContactEnumerationAlias);
            if (id != null)
                ContactsEnumerationModel.ContactID = id;
            if (shortcutID != null)
                ContactsEnumerationModel.ContactShortcutID = shortcutID;

            ContactsEnumerationModel.ShowContacts = type;

            if (blockContactsEnumeration == null)
            {
                ContactsEnumerationModel.AssignDefaultValues();
                ContactsEnumerationModel.Visible = true;
                ContactsEnumerationModel.Alias = ContactEnumerationAlias;
                ContactsEnumerationModel.Name = alias;
                
                ContactsEnumerationModel.FileSource = 2;//Entire Site
                ContactsEnumerationModel.SourceBlockAlias = ContactsEnumerationModel.Name;
                if (xml != null)
                {
                    ContactsEnumerationModel.Xml = xml;
                }
                blockContactsEnumeration = ContactsEnumerationModel.Apply(repository) as ContactEnumeration;
                repository.ApplyChanges();
                var columns = new List<GridColumn>();
                columns.Add(new GridColumn()
                {
                    Name = "Name",
                    Alignment = "Centered",
                    ShowInGrid = true,
                    ShowInDetail = false,
                    ShowAsLink = false,
                    OnlyForAuthorized = false,
                    Order = 0
                });
                columns.Add(new GridColumn()
                {
                    Name = "Type",
                    Alignment = "Centered",
                    ShowInGrid = true,
                    ShowInDetail = false,
                    ShowAsLink = false,
                    OnlyForAuthorized = false,
                    Order = 1
                });
                columns.Add(new GridColumn()
                {
                    Name = "Email",
                    Alignment = "Centered",
                    ShowInGrid = true,
                    ShowInDetail = false,
                    ShowAsLink = false,
                    OnlyForAuthorized = false,
                    Order = 2
                });
                ContactsEnumerationModel.ID = blockContactsEnumeration.BlockID;
                ContactsEnumerationModel.AssignColumns(columns);
                ContactsEnumerationModel.ApplyColumns(repository);
            }
            
            ContactsEnumerationModel.Load(repository,
                new BlockCommandModel(_settings) { BlockAlias = alias, ID = blockContactsEnumeration.BlockID }, needToLoadContent, "", TypeContextMenu.Contacts, contactProvider, type, xml);
            ContactsEnumerationModel.IsStatic = true;

            var blockContactDetails = repository.GetByAlias<ContactDetails>(ContactDetailsAlias);

            if (blockContactDetails == null)
            {
                ContactDetailsModel.AssignDefaultValues();
                ContactDetailsModel.Visible = true;
                
                ContactDetailsModel.Alias = ContactDetailsAlias;
                ContactDetailsModel.SourceBlockAlias = ContactsEnumerationModel.Name;
                blockContactDetails = ContactDetailsModel.Apply(repository) as ContactDetails;
                
                repository.ApplyChanges();
                ContactDetailsModel.ID = blockContactDetails.BlockID;
            }
            ContactDetailsModel.Load(repository,
                new BlockCommandModel(_settings) { BlockAlias = alias, ID = blockContactDetails.BlockID }, "",
                needToLoadContent);
            ContactDetailsModel.IsStatic = true;

        }

        public override void Update()
        {
           
        }

        public void Assign(BrowserContacts source)
        {
                base.Assign(source.Block);
            ContactEnumerationAlias = source.ContactEnumerationAlias;
            ContactDetailsAlias = source.ContactDetailsAlias;
        }
    }
}