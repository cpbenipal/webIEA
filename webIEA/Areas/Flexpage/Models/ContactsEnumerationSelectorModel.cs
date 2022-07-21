using Flexpage.Abstract;
using Flexpage.Code.StructureManagement;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Flexpage.Helpers;
using Flexpage.Helpers.StructureManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Flexpage.Models
{
    public class ContactsEnumerationSelectorModel : ViewModel
    {
        public string Title { get; set; }
        public ContactsEnumerationModel ContactsEnumerationModel { get; set; }

        public ContactsEnumerationSelectorModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            
            ContactsEnumerationModel = new ContactsEnumerationModel(settings, flexpage);
        }
        
        public override void FillViewData(ViewDataDictionary viewData, IFlexpageRepository repository,string Title="")
        {
            var alias = "_contactsEnumSelectorV9_" + (string.IsNullOrEmpty(Title) ? "" : Title.Replace('\\', ' '));
            this.Title = Title;
             var blockContactsEnumeration = repository.GetByAlias<ContactEnumeration>("_ContactsEnumeration" + alias);

            if (blockContactsEnumeration == null)
            {
                ContactsEnumerationModel.AssignDefaultValues();
                ContactsEnumerationModel.Visible = true;

                ContactsEnumerationModel.ShowSelectedContactsLabel = true;
                ContactsEnumerationModel.Alias = "_ContactsEnumeration" + alias;
                ContactsEnumerationModel.FilderFolder = Title;
                ContactsEnumerationModel.ShowSelectionBoxes = true;
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
                    Name = "ID",
                    Alignment = "Centered",
                    ShowInGrid = false,
                    ShowInDetail = false,
                    ShowAsLink = false,
                    OnlyForAuthorized = false,
                    Order = 0
                });
                columns.Add(new GridColumn()
                {
                    Name = "Type",
                    Alignment = "Centered",
                    ShowInGrid = false,
                    ShowInDetail = false,
                    ShowAsLink = false,
                    OnlyForAuthorized = false,
                    Order = 0
                });
                ContactsEnumerationModel.ID = blockContactsEnumeration.BlockID;
                ContactsEnumerationModel.AssignColumns(columns);
                ContactsEnumerationModel.ApplyColumns(repository);
            }
            ContactsEnumerationModel.Load(repository,
                new BlockCommandModel(_settings) { BlockAlias = alias, ID = blockContactsEnumeration.BlockID },
                "", true, true);
            ContactsEnumerationModel.IsStatic = true;
        }

        public override void Update()
        {
           
        }
        public void Assign(Maintenance source)
        {
           
        }
    }
}