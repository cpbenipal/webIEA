using DevExpress.Web.Mvc;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using Flexpage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.Shared.Abstract.DTO;
using Pluritech.Contact.Abstract;

namespace Flexpage.Models
{
    public enum ContactsEnumerationOrderBy
    {
        Field, Name, Type, Email
    };
    
    public class ContactsEnumerationModel : ContentModel
    {
        public string Name { get; set; }
        public eContactType ShowContacts { get; set; }
        public bool FilderLogin { get; set; }
        public string Xml { get; set; }
        public string FilderFolder { get; set; }
        public int? ContactID { get; set; }
        public int? ContactShortcutID { get; set; }
        public bool ShowSelectedContactsLabel { get; set; }
        public bool ShowSelectedContactsGrid { get; set; }
        public string SelectedValues { get; set; }
        public string Keywords { get; set; }

        IContactProvider _contactProvider { get; set; }

        public ContactsEnumerationModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : this(settings, flexpage, null) {
           
        }

        public ContactsEnumerationModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, NameValueCollection queryString)
            : base(settings, flexpage, queryString)
        {
            ContentType = ContentType.Contacts;
            ContentName = ContentName.ContactsEnumeration;
            ColMappings = new List<Abstract.DTO.ColumnMapping>() {
                new Abstract.DTO.ColumnMapping() { ShortName = "ID", FullName = "PersonCompany.ID", Caption = "ID" },
                new Abstract.DTO.ColumnMapping() { ShortName = "Title", FullName = "PersonCompany.Title", Caption = "Title" },
                new Abstract.DTO.ColumnMapping() { ShortName = "FirstName", FullName = "PersonCompany.FirstName", Caption = "First Name" },
                new Abstract.DTO.ColumnMapping() { ShortName = "LastName", FullName = "PersonCompany.LastName", Caption = "Last Name" },
                new Abstract.DTO.ColumnMapping() { ShortName = "Name", FullName = "PersonCompany.Name", Caption = "Full Name" },
                new Abstract.DTO.ColumnMapping() { ShortName = "Type", FullName = "PersonCompany.Type", Caption = "Type" },
                new Abstract.DTO.ColumnMapping() { ShortName = "Email", FullName = "PersonCompany.Email", Caption = "Email" },
                new Abstract.DTO.ColumnMapping() { ShortName = "Phone", FullName = "PersonCompany.Phone", Caption = "Phone" },
                new Abstract.DTO.ColumnMapping() { ShortName = "Language", FullName = "PersonCompany.LanguageName", Caption = "Language" },
                new Abstract.DTO.ColumnMapping() { ShortName = "Login", FullName = "PersonCompany.Login", Caption = "Login" },
                new Abstract.DTO.ColumnMapping() { ShortName = "TradeNumber", FullName = "PersonCompany.TradeNumber", Caption = "Trade Number" },
                new Abstract.DTO.ColumnMapping() { ShortName = "VAT", FullName = "PersonCompany.VAT", Caption = "VAT" },
                new Abstract.DTO.ColumnMapping() { ShortName = "MainAddress", FullName = "PersonCompany.MainAddress", Caption = "Main address" },
            };            
            AllowedOrderBy = System.Enum.GetNames(typeof(ContactsEnumerationOrderBy));
            if (queryString != null)
            {
                Keywords = queryString["keywords"];
            }
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent=true)
        {
            Load(repository, proto, needToLoadContent, null, TypeContextMenu.Block, null, eContactType.None,null);
        }

        public void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true, bool showSelectedContacts = false)
        {
            Load(repository, proto, needToLoadContent, null, TypeContextMenu.Block, null, eContactType.None, null, showSelectedContacts);
        }
        private void LoadAdditionalColumns(ObjectTypeEnum type)
        {
            var filesQueryBuilder = new FilesQueryBuilder();
            var objectTypes = filesQueryBuilder.GetObjectType(type);

            foreach(var objectType in objectTypes)
            {
                ColMappings.Add(new Abstract.DTO.ColumnMapping()
                {
                    ShortName = objectType.Name,
                    FullName = "PersonShortcut." + objectType.Name,
                    Caption = objectType.Caption
                });
            }
        }

        public void Load(IFlexpageRepository repository, BlockCommandModel proto, bool needToLoadContent, string SelectFolderName, TypeContextMenu typeContextMenu= TypeContextMenu.Block, 
            IContactProvider contactProvider=null, eContactType ShowContacts = eContactType.None, string xml = null, bool showSelectedContacts = false, string selectedValues = "")
        {
            base.Load(repository, proto,"", needToLoadContent);
            Repository = repository;

            ShowSelectedContactsGrid = showSelectedContacts;
            SelectedValues = selectedValues;

            if(PWBrowser)
            {
                LoadAdditionalColumns(ObjectTypeEnum.PersonShortcut);
                LoadAdditionalColumns(ObjectTypeEnum.CompanyShortcut);
            }
            else
            {
                LoadAdditionalColumns(ObjectTypeEnum.PersonObject);
                LoadAdditionalColumns(ObjectTypeEnum.CompanyObject);
            }

            ContactEnumeration block = repository.GetByBlockID<ContactEnumeration>(proto.ID);
            if(block == null)
                AssignDefaultValues();
            else
            {
                Repository = repository;
                Assign(block);
                if(needToLoadContent)
                {
                    //GetDataObjects(repository);
                }
            }

            this.SelectFolderName = SelectFolderName;
            if (block != null)
            {
                var folder = repository.GetFolder(block.FolderName);
                this.SelectFolderId = folder?.ID ?? 0;
            }
            TypeContextMenu = typeContextMenu;
            this.Xml = xml;

            this.ShowContacts = ShowContacts;
            ShowSelectedContactsLabel = showSelectedContacts; 

            _contactProvider = contactProvider;            

        }
                
        public override void AssignDefaultValues(params object[] args)
        {
            base.AssignDefaultValues();
            SourceType = 1;
        }
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            ContactEnumeration contactEnumeration = repository.GetByBlockID<ContactEnumeration>(ID);
            if (contactEnumeration == null || Alias == repository.CreateNewAlias) // ???
            {
                if (Alias == repository.CreateNewAlias)
                    Alias = null;

                contactEnumeration = repository.CreateNewContactEnumeration(Visible, CssClass,Alias);
                repository.AddBlockToBlockList(contactEnumeration.Block, BlocklistID, BlockAfter);
            }

            //apply props
            
            //folderContent.ShowSize = ShowSize;
            contactEnumeration.Name = Name;
            contactEnumeration.OrderBy = OrderBy;
            contactEnumeration.OrderDirection = OrderDirection;
            contactEnumeration.OrderFieldName = OrderFieldName;
            contactEnumeration.GroupingSetCurrentLanguageFirst = GroupingSetCurrentLanguageFirst;
            contactEnumeration.GroupByDescription = GroupByDescription;
            //folderContent.ShowItemAsLink = ShowItemAsLink;
            contactEnumeration.ShowAllLanguagesInLine = ShowAllLanguagesInLine;
            contactEnumeration.ShowMoreLanguagesText = ShowMoreLanguagesText;
            contactEnumeration.EnableRestrictLanguage = EnableRestrictLanguage;
            contactEnumeration.PagingSize = PagingSize;
            contactEnumeration.ShowDetailRow = ShowDetailRow;
            contactEnumeration.AllowOnlyOneDetailRow = AllowOnlyOneDetailRow;
            contactEnumeration.ExpandAllDetailsOnLoad = ExpandAllDetailsOnLoad;
            contactEnumeration.AllowSort = AllowSort;
            contactEnumeration.ShowFilterRow = ShowFilterRow;
            contactEnumeration.ShowHeaderRow = ShowHeaderRow;
            contactEnumeration.ShowContacts = ShowContacts;
            contactEnumeration.AllowExport = AllowExport;

            //TODO

            contactEnumeration.DateFormat = DateFormat;
            contactEnumeration.ShowDetailButtons = ShowDetailButtons;
            contactEnumeration.ShowClearColumn = ShowClearColumn;
            //folderContent.UseFilter = UseFilter;
            contactEnumeration.Filter = Filter;
            contactEnumeration.SourceType = SourceType==1 && WithParameter ? 2: SourceType;
            contactEnumeration.FolderName = PWFolderName;
            contactEnumeration.SourceBlockAlias = (SourceType == 0 && !string.IsNullOrEmpty(SourceBlockAlias)) ? SourceBlockAlias : (SourceType == 0 ? contactEnumeration.SourceBlockAlias : ""); ;
            contactEnumeration.LastDocQnty = LastDocQnty;
            contactEnumeration.FileSource = SourceType == 3?2: FileSource;
            contactEnumeration.SourceCriteria = SourceCriteria;

            contactEnumeration.ShowSearchControl = ShowSearchControl;
            contactEnumeration.FilderLogin = FilderLogin;
            contactEnumeration.FilderFolder = FilderFolder;
            contactEnumeration.ShowSelectionBoxes = ShowSelectionBoxes;
            repository.ApplyChanges();

            ID = contactEnumeration.BlockID;
            ApplyColumns(repository);

            
            return contactEnumeration;
        }

        public override void Assign(object source, params object[] args)
        {
            ContactEnumeration contactEnumeration = source as ContactEnumeration;
            base.Assign(contactEnumeration.Block);

            Name = contactEnumeration.Name;
            OrderBy = contactEnumeration.OrderBy;
            OrderDirection = contactEnumeration.OrderDirection;
            OrderFieldName = contactEnumeration.OrderFieldName;
            GroupingSetCurrentLanguageFirst = contactEnumeration.GroupingSetCurrentLanguageFirst ?? false;
            GroupByDescription = contactEnumeration.GroupByDescription ?? false;
            ShowAllLanguagesInLine = contactEnumeration.ShowAllLanguagesInLine ?? false;
            ShowMoreLanguagesText = contactEnumeration.ShowMoreLanguagesText ?? false;
            EnableRestrictLanguage = contactEnumeration.EnableRestrictLanguage ?? false;
            PagingSize = contactEnumeration.PagingSize;
            ShowDetailRow = contactEnumeration.ShowDetailRow;
            AllowOnlyOneDetailRow = contactEnumeration.AllowOnlyOneDetailRow;
            ExpandAllDetailsOnLoad = contactEnumeration.ExpandAllDetailsOnLoad;
            AllowExport = contactEnumeration.AllowExport;
            AllowSort = contactEnumeration.AllowSort;
            ShowFilterRow = contactEnumeration.ShowFilterRow;
            ShowHeaderRow = contactEnumeration.ShowHeaderRow;
            DateFormat = contactEnumeration.DateFormat;
            ShowDetailButtons = contactEnumeration.ShowDetailButtons;
            ShowClearColumn = contactEnumeration.ShowClearColumn;
            Filter = contactEnumeration.Filter;
            LastDocQnty = contactEnumeration.LastDocQnty ?? 10;
            ShowItemAsLink = contactEnumeration.ShowItemAsLink ?? false;
            FileSource = contactEnumeration.FileSource ?? 0;
            SourceCriteria = contactEnumeration.SourceCriteria;
            SourceType = contactEnumeration.SourceType;
            PWFolderName = contactEnumeration.FolderName;
            SourceBlockAlias = SourceType == 0 ? contactEnumeration.SourceBlockAlias : "";
            WithParameter = SourceType == 2;
            ShowContacts = contactEnumeration.ShowContacts;
            ShowSearchControl = contactEnumeration.ShowSearchControl ?? false;
            FilderLogin = contactEnumeration.FilderLogin;
            FilderFolder = contactEnumeration.FilderFolder;
            ShowSelectionBoxes= contactEnumeration.ShowSelectionBoxes;
            
            if (contactEnumeration.Block.GridColumns.Any())
            {
                AssignColumns(contactEnumeration.Block.GridColumns);              
            }
            GridModel.Pager.PageSize = PagingSize;
            CardModel.Pager.SettingsTableLayout.ColumnCount = PagingSize;

            //TODO: Set from forms controls
            CardModel.Pager.SettingsTableLayout.RowsPerPage = 1;

        }


        public override List<OrderByDesciption> FileSourceOrderBy(List<OrderByDesciption> orderBy)
        {
            if (ContentType == ContentType.File)
            {
                if(FileSource == 2|| SourceType == 3)// Entire Site
                {
                    if(SourceCriteria == "EntryDate")
                    {
                        orderBy.Add(new OrderByDesciption()
                        {
                            Table = "CustomProperty",
                            Field = "EntryDate",
                            Ascending = false
                        });
                    }
                    else
                    {

                    }
                }
            }
            return orderBy;
        }
        protected override List<FilterDesciption> GenerateFilterBy(string filterExpression, string defTable, string treeList_FolderName = null, List<FilterDesciption> filterBy = null)
        {
            filterBy = new List<FilterDesciption>();
            filterBy.Add(new FilterDesciption() { Table= "ShowContacts",  Value= (int)ShowContacts });
            return base.GenerateFilterBy(filterExpression, defTable, treeList_FolderName, filterBy);
        }
        public override void GetDataObjects(GridViewCustomBindingGetDataArgs e)
        {
            GetDataObjects(e,null,null);
        }
        public void GetDataObjects(GridViewCustomBindingGetDataArgs e, int? contactID, int? contactShortcutID, eContactType ShowContacts = eContactType.None)
        {
            var defTable = "PersonCompany";
            this.ShowContacts = ShowContacts;
            var filterBy = ApplyFilter(e.FilterExpression, defTable, contactID, contactShortcutID);
            if (ShowSelectedContactsGrid && !String.IsNullOrEmpty(SelectedValues))
            {
                var values = SelectedValues.Split(',').Select(v => int.Parse(v)).ToList();
                filterBy.Add(new FilterDesciption() { Table = defTable, Field = "ShortcutID", Value = values });
            }
            GetDataObjects(e, defTable, Repository.QueryContacts, SelectFolderName, filterBy);
        }
        public override void GetDataObjectsCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            GetDataObjectsCount(e,null,null);
        }
        public void GetDataObjectsCount(GridViewCustomBindingGetDataRowCountArgs e, int? contactID, int? contactShortcutID, eContactType ShowContacts = eContactType.None)
        {
            var defTable = "PersonCompany";
            this.ShowContacts = ShowContacts;
            var filterBy = ApplyFilter(e.FilterExpression, defTable, contactID, contactShortcutID);
            if (ShowSelectedContactsGrid && !String.IsNullOrEmpty(SelectedValues))
            {
                var values = SelectedValues.Split(',').Select(v => int.Parse(v)).ToList();
                filterBy.Add(new FilterDesciption() { Table = defTable, Field = "ShortcutID", Value = values });
            }
            GetDataObjectsCount(e, defTable, Repository.QueryContacts, SelectFolderName, filterBy);
        }
          

        private List<FilterDesciption> ApplyFilter(string filterExpression, string defTable, int? contactID, int? contactShortcutID)
        {
            var filterBy = GenerateFilterBy(filterExpression, defTable, SelectFolderName);

            if (FilderLogin)
                filterBy.Add(new FilterDesciption() { Table = defTable, Field = "Login" });

            if (contactID != null)
                filterBy.Add(new FilterDesciption() { Table = defTable, Field = "ID", Value = contactID });

            if (contactShortcutID != null)
                filterBy.Add(new FilterDesciption() { Table = defTable, Field = "ShortcutID", Value = contactShortcutID });

            if (Xml != null)
            {
                Xml = HttpUtility.UrlDecode(Xml);
                var res = _contactProvider.SearchContacts(Xml);
                var contacts = res.ToList();
                var type = _contactProvider.GetTypeXML(Xml);
                filterBy.Add(new FilterDesciption()
                {
                    Field = type.ToString(),
                    Table = "SearchResult",
                    Value = contacts
                });
            }
            
            return filterBy;
        }
    }

    public class ContactsEnumerationJson
    {
        public int ContactID { get; set; }

        public eContactType ContactType { get; set; }

        public string Path { get; set; }

        public string Command { get; set; }
    }

}