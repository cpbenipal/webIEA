using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
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

namespace Flexpage.Models
{
    public class BrowserModel : BlockModel
    {
        public FolderTreeListModel FolderTreeListModel { get; set; }
        public FolderContentModel FolderContentModel { get; set; }
        public ContactsEnumerationModel ContactsEnumerationModel { get; set; }
        public string SelectFolderName { get; set; }

        /// <summary>
        /// Added to keep last selected file or to command browser to select certain file at startup
        /// </summary>
        public int? SelectedFileID { get; set; } 
        public string FolderTreeListAlias { get; set; }
        public string FolderContentAlias { get; set; }
        public string ContactEnumerationAlias { get; set; }
        public bool Basic { get; set; } = false;
        public bool Contacts { get; set; } = true;
        public bool ContextMenu { get; set; } = true;
        public bool HyperlinkFiles { get; set; } = true;
        public bool UploadFiles { get; set; } = true;
        public bool BrowserSearch { get; set; } = true;
        public BrowserModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            FolderTreeListModel = new FolderTreeListModel(settings, flexpage);
            FolderTreeListModel.PWBrowser = true;
            FolderContentModel = new FolderContentModel(settings, flexpage);
            FolderContentModel.PWBrowser = true;
            ContactsEnumerationModel = new ContactsEnumerationModel(settings, flexpage);
        }
        public BrowserModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, bool contacts, bool contextMenu,bool hyperlinkFiles,bool uploadFiles, bool browserSearch) 
            : this(settings, flexpage)
        {
            Contacts = contacts;
            ContextMenu = contextMenu;
            HyperlinkFiles = hyperlinkFiles;
            UploadFiles = uploadFiles;
            BrowserSearch = browserSearch;
        }
        public BrowserModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, bool basic)
            : this(settings, flexpage, !basic, !basic, !basic, !basic, !basic)
        {
            Basic = basic;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);
            Browser target = repository.GetByBlockID<Browser>(ID);

            if (target == null)
            {
                target = repository.CreateNewBrowser();
                if (BlocklistID > -1)
                    repository.AddBlockToBlockList(target.Block, BlocklistID, BlockAfter);
                repository.ApplyChanges();
                target.Block.Alias = string.Format("_browser_{0}", target.BlockID);
                repository.ApplyChanges();
            }

            target.BrowserSearch = BrowserSearch;
            target.UploadFiles = UploadFiles;
            target.ContextMenu = ContextMenu;
            target.HyperlinkFiles = HyperlinkFiles;
            target.Contacts = Contacts;
            target.ContactEnumerationAlias = ContactEnumerationAlias;
            target.FolderContentAlias = FolderContentAlias;
            target.FolderTreeListAlias = FolderTreeListAlias;

            return null;
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            ID = proto.ID;
            Load(repository, proto.BlockAlias, needToLoadContent, false,false);
        }

        public void Load(IFlexpageRepository repository, string alias, bool needToLoadContent = true, bool AllowSelectByRowClick = false, bool SaveSelectedNode=false)
        {
            var source = repository.GetByAlias<Browser>(alias);

            if (source != null)
            {
                Assign(source);
            }
            alias = "_browser_" + alias;
            
            FolderTreeListAlias = "_FolderTreeList" + (UploadFiles ? "" : "_NoUF") + alias;
            FolderContentAlias = "_FolderContent" + (HyperlinkFiles ? "" : "_NoH") + (UploadFiles ? "" : "_NoUF") + alias;
            ContactEnumerationAlias = "_ContactsEnumeration" + alias;

            var blockFolderTreeList = repository.GetByAlias<FolderTreeList>(FolderTreeListAlias);
            if (blockFolderTreeList == null)
            {
                FolderTreeListModel.AssignDefaultValues();
                FolderTreeListModel.Visible = true;
                FolderTreeListModel.Alias = FolderTreeListAlias;
                FolderTreeListModel.Name = alias;
                FolderTreeListModel.SaveSelectedNode = SaveSelectedNode;
                blockFolderTreeList = FolderTreeListModel.Apply(repository) as FolderTreeList;
                FolderTreeListModel.ID = blockFolderTreeList.BlockID;
                if (!UploadFiles)
                {
                    blockFolderTreeList.AllowUpload = false;
                }
                repository.ApplyChanges();
                FolderTreeListModel.ID = blockFolderTreeList.BlockID;
            }
            FolderTreeListModel.Load(repository,
                    new BlockCommandModel(_settings) { BlockAlias = alias, ID = blockFolderTreeList.BlockID }, "",
                    needToLoadContent);
            FolderTreeListModel.IsStatic = true;
            if (!ContextMenu)
            {
                FolderTreeListModel.TypeContextMenu = TypeContextMenu.None;
            }
            else
            {
                FolderTreeListModel.TypeContextMenu = TypeContextMenu.Browser;
            }

            var blockFolderContent = repository.GetByAlias<FolderContent>(FolderContentAlias);
            if (blockFolderContent == null)
            {
                FolderContentModel.AllowSelectByRowClick = AllowSelectByRowClick;
                FolderContentModel.AssignDefaultValues();
                FolderContentModel.Visible = true;
                FolderContentModel.Alias = FolderContentAlias;
                FolderContentModel.SourceBlockAlias = FolderTreeListModel.Name;
                blockFolderContent = FolderContentModel.Apply(repository) as FolderContent;
                if (!UploadFiles)
                {
                    blockFolderContent.AllowUpload = false;
                }
                repository.ApplyChanges();
                var columns = new List<GridColumn>();
                if (HyperlinkFiles)
                {
                    columns.Add(new GridColumn()
                    {
                        Name = "Date",
                        Alignment = "Centered",
                        ShowInGrid = true,
                        ShowInDetail = false,
                        ShowAsLink = false,
                        OnlyForAuthorized = false,
                        Order = 0
                    });
                    columns.Add(new GridColumn()
                    {
                        Name = "Extension",
                        Alignment = "Centered",
                        ShowInGrid = true,
                        ShowInDetail = false,
                        ShowAsLink = false,
                        OnlyForAuthorized = false,
                        Order = 1
                    });
                    columns.Add(new GridColumn()
                    {
                        Name = "Name",
                        Alignment = "Centered",
                        ShowInGrid = true,
                        ShowInDetail = false,
                        ShowAsLink = true,
                        OnlyForAuthorized = false,
                        Order = 2
                    });
                }
                else
                {
                    columns.Add(new GridColumn()
                    {
                        Name = "Date",
                        Alignment = "Centered",
                        ShowInGrid = true,
                        ShowInDetail = false,
                        ShowAsLink = false,
                        OnlyForAuthorized = false,
                        Order = 0
                    });
                    columns.Add(new GridColumn()
                    {
                        Name = "Extension",
                        Alignment = "Centered",
                        ShowInGrid = true,
                        ShowInDetail = false,
                        ShowAsLink = false,
                        OnlyForAuthorized = false,
                        Order = 1
                    });
                    columns.Add(new GridColumn()
                    {
                        Name = "Name",
                        Alignment = "Centered",
                        ShowInGrid = true,
                        ShowInDetail = false,
                        ShowAsLink = false,
                        OnlyForAuthorized = false,
                        Order = 2
                    });
                }
                FolderContentModel.ID = blockFolderContent.BlockID;
                FolderContentModel.AssignColumns(columns);
                FolderContentModel.ApplyColumns(repository);
                
            }
            FolderContentModel.Load(repository,
                    new BlockCommandModel(_settings) { BlockAlias = alias, ID = blockFolderContent.BlockID }, needToLoadContent, "\\");
            FolderContentModel.IsStatic = true;
            if (!ContextMenu)
            {
                FolderContentModel.TypeContextMenu = TypeContextMenu.None;
            }
            else
            {
                FolderContentModel.TypeContextMenu = TypeContextMenu.Browser;
            }

            if (FolderTreeListModel.SaveSelectedNode && System.Web.HttpContext.Current.Session[FolderTreeListModel.SaveSelectedKey] != null)
            {
                string _sessionVal = System.Web.HttpContext.Current.Session[FolderTreeListModel.SaveSelectedKey].ToString();
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(_sessionVal);
                SelectFolderName = json["path"];
            }
            else
            {
                SelectFolderName = FolderTreeListModel.GetChildren(null).FirstOrDefault()?.FullName;
            }

            FolderContentModel.SelectFolderName = SelectFolderName;
            FolderContentModel.UpdateFolderPermissions();
            if (Contacts)
            {
                var blockContactsEnumeration = repository.GetByAlias<ContactEnumeration>(ContactEnumerationAlias);
                if (blockContactsEnumeration == null)
                {
                    ContactsEnumerationModel.AssignDefaultValues();
                    ContactsEnumerationModel.Visible = true;
                    ContactsEnumerationModel.Alias = ContactEnumerationAlias;
                    ContactsEnumerationModel.SourceBlockAlias = FolderTreeListModel.Name;
                    blockContactsEnumeration = ContactsEnumerationModel.Apply(repository) as ContactEnumeration;
                    blockContactsEnumeration.SourceType = 0;
                    ContactsEnumerationModel.SourceType = 0;
                    repository.ApplyChanges();
                    var columns = new List<GridColumn>();
                    columns.Add(new GridColumn()
                    {
                        Name = "Name",
                        Alignment = "Centered",
                        ShowInGrid = true,
                        ShowInDetail = false,
                        ShowAsLink = true,
                        OnlyForAuthorized = false,
                        Order = 0
                    });
                    ContactsEnumerationModel.ID = blockContactsEnumeration.BlockID;
                    ContactsEnumerationModel.AssignColumns(columns);
                    ContactsEnumerationModel.ApplyColumns(repository);
                }
                ContactsEnumerationModel.Load(repository,
                    new BlockCommandModel(_settings) { BlockAlias = alias, ID = blockContactsEnumeration.BlockID }, needToLoadContent, "");
                ContactsEnumerationModel.IsStatic = true;
                if (!ContextMenu)
                {
                    FolderContentModel.TypeContextMenu = TypeContextMenu.None;
                }
                else
                {
                    ContactsEnumerationModel.TypeContextMenu = TypeContextMenu.Browser;
                }
            }
        }

        public override void Update()
        {
           
        }
        public void Assign(Browser source)
        {
            base.Assign(source.Block);
            Basic = source.Basic;
            Contacts = source.Contacts;
            ContextMenu = source.ContextMenu;
            HyperlinkFiles = source.HyperlinkFiles;
            UploadFiles = source.UploadFiles;
            BrowserSearch = source.BrowserSearch;
            FolderContentAlias = source.FolderContentAlias;
            FolderTreeListAlias = source.FolderTreeListAlias;
            ContactEnumerationAlias = source.ContactEnumerationAlias;
        }
    }
}