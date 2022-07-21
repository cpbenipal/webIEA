using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;

namespace Flexpage.Models
{
    public class PermitRuleModel : ViewModel
    {
        public ContactsEnumerationModel ContactsEnumerationModel { get; set; }
        public List<Folder> Folders { get; set; }

        public string TypeContent { get; set; }

        public PermitRuleModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Folders = new List<Folder>();
            ContactsEnumerationModel = new ContactsEnumerationModel(settings, flexpage);
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title,
            bool needToLoadContent = true)
        {
            ID = proto.ID;
            TypeContent = proto.Parameters;

            if (proto.Parameters.Equals("AddGroup"))
                Folders.AddRange(repository.GetFoldersSecurityGroup());
            else
            {
                var alias = "_ObjectPropertiesVer2_";
                var blockContactsEnumeration =
                    repository.GetByAlias<ContactEnumeration>("_ContactsEnumeration" + alias);

                if (blockContactsEnumeration == null)
                {
                    ContactsEnumerationModel.AssignDefaultValues();
                    ContactsEnumerationModel.Visible = true;
                    ContactsEnumerationModel.Alias = "_ContactsEnumeration" + alias;
                    ContactsEnumerationModel.ShowContacts = Pluritech.Contact.Abstract.DTO.eContactType.Person;
                    blockContactsEnumeration = ContactsEnumerationModel.Apply(repository) as ContactEnumeration;
                    blockContactsEnumeration.Block.GridColumns.Add(new GridColumn()
                    {
                        Name = "Name",
                        Alignment = "Centered",
                        ShowInGrid = true,
                        ShowInDetail = false,
                        ShowAsLink = true,
                        OnlyForAuthorized = false,
                        Order = 0
                    });
                    repository.ApplyChanges();
                    ContactsEnumerationModel.ID = blockContactsEnumeration.BlockID;
                }

                ContactsEnumerationModel.Load(repository,
                    new BlockCommandModel(_settings)
                        {BlockAlias = proto.BlockAlias, ID = blockContactsEnumeration.BlockID},
                    "",
                    needToLoadContent);
                ContactsEnumerationModel.IsStatic = true;
            }
        }
    }

    public class NewDataObject
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public int PermissionId { get; set; }

        public bool IsDeny { get; set; }

        public string Parameters { get; set; }

        public string Name { get; set; }

        public bool IsRemoved { get; set; }

        public string LastAction { get; set; }
    }
}