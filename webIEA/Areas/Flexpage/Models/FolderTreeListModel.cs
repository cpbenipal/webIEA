using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using Flexpage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Web;
using Flexpage.Code.Helpers;
using Flexpage.Domain.Enum;
using DevExpress.Web.ASPxTreeList;

using Pluritech.Shared.Abstract.DTO;
using Pluritech.Shared.Abstract.DTO.Extensions;
using Pluritech.Shared.Abstract;
using Pluritech.Permissions.Abstract.DTO;

namespace Flexpage.Models
{

    public class FolderTreeListModel : BlockModel
    {
        public string Name { get; set; }

        public string PWFolderName { get; set; }

        public int RootFolder { get; set; }

        public int? CountToShow { get; set; }

        public ShowTypeLevel ShowType { get; set; }

        public bool ViewRights { get; set; }

        public FolderTreeListGridLines GridLines { get; set; }

        public bool ShowTreeLines { get; set; }

        public bool UseDescrAsName { get; set; }

        public bool SortShownNames { get; set; }

        public bool ShowRootNode { get; set; }

        public string ExpandImage { get; set; }

        public string CollapseImage { get; set; }

        public MVCxTreeViewNodeCollection Nodes { get; set; }

        public string NoExpandImage { get; set; }

        public IFlexpageRepository Repository { get; set; }

        public bool AllowUpload { get; set; }

        public bool EnableNotifications { get; set; }

        public bool PWBrowser { get; set; }

        public bool SaveSelectedNode { get; set; }
        
        public int? SelectedFolderID{ get; set; }

        public bool IsPWAdminMode { get; set; }

        private int? UserID { get; set; }
        public readonly string SaveSelectedKeyBase = "nodeSelected_fp_FolderTreeList_List";
        private string _saveSelectedKey { get; set; } 
        public string SaveSelectedKey
        {
            get
            {
                return _saveSelectedKey;
            }
            set
            {
                _saveSelectedKey = value;
            }
        }
        public TypeContextMenu TypeContextMenu { get; set; }

        public FolderTreeListModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage) {
           

        }

        public override void AssignDefaultValues(params object[] args)
        {
            PWFolderName = "\\";
            Name = "";
            ShowRootNode = true;
            UseDescrAsName = true;
            SortShownNames = true;
            ShowTreeLines = true;
            SaveSelectedNode = false;
            _saveSelectedKey= SaveSelectedKeyBase + ID;
        }
        public override void Assign(object source, params object[] args)
        {
            FolderTreeList folderTreeList = source as FolderTreeList;
            base.Assign(folderTreeList.Block);

            PWFolderName = folderTreeList.FolderName;
            CountToShow = folderTreeList.CountToShow;
            Name = folderTreeList.Name ?? "";
            RootFolder = folderTreeList.RootFolder;
            ShowRootNode = folderTreeList.ShowRootNode;
            ShowTreeLines = folderTreeList.ShowTreeLines;
            ShowType = folderTreeList.ShowType;
            SortShownNames = folderTreeList.SortShownNames;
            UseDescrAsName = folderTreeList.UseDescrAsName;
            EnableNotifications = folderTreeList.EnableNotifications;
            ViewRights = folderTreeList.ViewRights;
            NoExpandImage = folderTreeList.NoExpandImage;
            ExpandImage = folderTreeList.ExpandImage;
            CollapseImage = folderTreeList.CollapseImage;
            AllowUpload = folderTreeList.AllowUpload;
            SaveSelectedNode = folderTreeList.SaveSelectedNode;
            if (_saveSelectedKey == null)
                _saveSelectedKey = SaveSelectedKeyBase + ID;
            switch (folderTreeList.GridLines)
            {
                case "none":
                    GridLines = FolderTreeListGridLines.None;
                    break;
                case "horizontal":
                    GridLines = FolderTreeListGridLines.Horizontal;
                    break;
                case "vertical":
                    GridLines = FolderTreeListGridLines.Vertical;
                    break;
                case "both":
                    GridLines = FolderTreeListGridLines.Both;
                    break;
                default:
                    break;
            }
            

        }
        
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent=true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            Repository = repository;
            FolderTreeList block = repository.GetByBlockID<FolderTreeList>(proto.ID);
            if (block == null) {
                AssignDefaultValues();
            }
            else
            {
                Assign(block);
            }

            IsPWAdminMode = _flexpageProcessor.IsPWAdmin();
        }

        public override void Update()
        {
        }


        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            FolderTreeList folderTreeList = repository.GetByBlockID<FolderTreeList>(ID);
            if (folderTreeList == null || Alias == repository.CreateNewAlias) // ???
            {
                if (Alias == repository.CreateNewAlias)
                    Alias = null;

                folderTreeList = repository.CreateNewFolderTreeList(Visible, CssClass, Alias);
                repository.AddBlockToBlockList(folderTreeList.Block, BlocklistID, BlockAfter);
            }
            folderTreeList.FolderName = this.PWFolderName;
            folderTreeList.CountToShow = this.CountToShow;
            folderTreeList.Name = Name ?? "";
            folderTreeList.RootFolder = this.RootFolder;
            folderTreeList.ShowRootNode = this.ShowRootNode;
            folderTreeList.ShowTreeLines = this.ShowTreeLines;
            folderTreeList.ShowType = this.ShowType;
            folderTreeList.SortShownNames = this.SortShownNames;
            folderTreeList.UseDescrAsName = this.UseDescrAsName;
            folderTreeList.EnableNotifications = this.EnableNotifications;
            folderTreeList.ViewRights = this.ViewRights;
            folderTreeList.NoExpandImage = this.NoExpandImage;
            folderTreeList.ExpandImage = this.ExpandImage;
            folderTreeList.CollapseImage = this.CollapseImage;
            folderTreeList.AllowUpload = AllowUpload;
            folderTreeList.GridLines = this.GridLines.GetDisplay().ToLower();
            folderTreeList.SaveSelectedNode = this.SaveSelectedNode;
            repository.ApplyChanges();
            return folderTreeList;
        }

        public List<ObjectInfo> GetDataObjects( string defTable, IObjectQueryBuilder query)
        {
            var result = query;
            return result.ObjectsList(true);
        }

        public void GetDataObjects()
        {
            string defTable = "FileShortcut";

            GetDataObjects(defTable, Repository.QueryFolders);
        }

        private void TransformToFolderInfo(List<ObjectInfo> source, List<FolderInfo> folders, string parentName, int parentID)
        {
            foreach(ObjectInfo folder in source.ToList())
            {
                PropertyInfo prop = folder.Properties.FirstOrDefault(p => p.Name == "Folder.Name");
                if(prop == null)
                {
                    source.Remove(folder);
                    continue;
                }
                string folderName = prop.Value.ToString();
                if(folderName.StartsWith(parentName))
                {
                    string[] parts = folderName.Substring(parentName.Length).Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                    var customProperties = folder.Properties.Where(p => p.Name.Contains("Images."));
                    var permProperties = folder.GetPropertyValue("Folder.Permissions");
                    if (ViewRights && ((short)permProperties & (short)ePermissions.View) > 0 || ViewRights == false)
                    {
                        var exportContactPermission = new List<string>()
                        {
                            "CustomProperty.DistributionList", "CustomProperty.CategoryGroup", "CustomProperty.PublishingFolder", "CustomProperty.SecurityGroup"
                        };
                        var ShowContactProperty = new List<string>()
                        {
                            "CustomProperty.DistributionList", "CustomProperty.CategoryGroup", "CustomProperty.SecurityGroup","CustomProperty.PublishingFolder"
                        };
                        var ShowNotifications = new List<string>()
                        {
                            "CustomProperty.PublishingFolder"
                        };
                        var cProperty = folder.Properties.Where(w => exportContactPermission.Contains(w.Name.Trim()))
                            .ToList();
                        var contactProperty = folder.Properties.Where(w => ShowContactProperty.Contains(w.Name.Trim()))
                            .ToList();
                        var notificationsProperty = folder.Properties.Where(w => ShowNotifications.Contains(w.Name.Trim()))
                           .ToList();
                        if (parts.Length == 1)
                        {
                        
                            folders.Add(new FolderInfo()
                            {
                                ID = folder.ID,
                                Name = parts[0],
                                ParentID = parentID,
                                FullName = folderName,
                                Level = Convert.ToInt32(folder.GetPropertyValue("Folder.Level")),
                                HasChilds = Convert.ToBoolean(folder.GetPropertyValue("Folder.HasChilds")),
                                ImgPaths = customProperties.Select(c => c.Name.Replace("Images.", "")).ToList(),
                                Permissions = permProperties != null ? Convert.ToInt32(permProperties) : 0,
                                IsExportContact = cProperty.Count > 0,
                                ShowContacts= contactProperty.Count>0,
                                ShowNotifications = notificationsProperty.Count > 0
                            });
                        }
                        else if(ShowRootNode && folderName.Equals(parentName))
                        {                        
                            parts = folderName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                            folders.Add(new FolderInfo()
                            {
                                ID = folder.ID,
                                Name = parts[parts.Length-1],
                                ParentID = parentID,
                                FullName = folderName,
                                Level = Convert.ToInt32(folder.GetPropertyValue("Folder.Level")),
                                HasChilds = Convert.ToBoolean(folder.GetPropertyValue("Folder.HasChilds")),
                                ImgPaths = customProperties.Select(c => c.Name.Replace("Images.", "")).ToList(),
                                Permissions = permProperties != null ? Convert.ToInt32(permProperties) : 0,
                                IsExportContact = cProperty.Count > 0,
                                ShowContacts = contactProperty.Count > 0,
                                ShowNotifications = notificationsProperty.Count > 0
                            });
                        }
                    }
                }
                else
                    if(!string.IsNullOrEmpty(parentName))
                    return;
            }
        }

        public void VirtualModeCreateChildren(TreeListVirtualModeCreateChildrenEventArgs e)
        {
            e.Children = GetChildren(e.NodeObject as FolderInfo);
        }
         public List<FolderInfo> GetChildren(FolderInfo FolderInfoParrent)
        {
            if (PWFolderName == null)
            {
                PWFolderName = "\\";
            }

            List<FolderInfo> folders = new List<FolderInfo>();
            var result = Repository.QueryFolders;
            string parent = FolderInfoParrent == null ? PWFolderName : (FolderInfoParrent as FolderInfo).FullName;
            var parretLevel = parent.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Count() - 2;
            var childLevel = parent.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Count() - 1;

            parretLevel = parretLevel == -2 ? -1 : parretLevel;

            int level = FolderInfoParrent == null ? (ShowRootNode ? parretLevel : childLevel) : FolderInfoParrent.Level;

            if (CountToShow > RootFolder && CountToShow < level + 1)
            {
                return folders;
            }
            else
            {
                //get child nodes
                var orderBy = new List<OrderByDesciption>()
                {
                    new OrderByDesciption() { Table = "Folder", Field = "Name" }
                };
                result = result.OrderBy(orderBy);
                var filterBy = new List<FilterDesciption>() {
                    new FilterDesciption()
                    {
                        Table = "Folder",
                        Field = "Name",
                        Value = parent,
                        FilterMode = eFilterMode.StartWith
                    },
                    new FilterDesciption()
                    {
                        Table = "Folder",
                        Field = "Level",
                        Value = level,
                        FilterMode = eFilterMode.GreatAt1
                    }
                };


                result = result.FilterBy(filterBy);                
                TransformToFolderInfo(result.ObjectsList(true), folders, parent, FolderInfoParrent?.ID??0);
                if (ShowRootNode && FolderInfoParrent == null && PWFolderName == "\\")
                {
                    return  new List<FolderInfo>() { getRootFolder(folders.Count) };
                }
                else
                {
                    return folders;
                }
            }
        }
        public void VirtualModeNodeCreating(TreeListVirtualModeNodeCreatingEventArgs e)
        {
            FolderInfo fi = e.NodeObject as FolderInfo;
            e.NodeKeyValue = fi.ID;
            e.IsLeaf = !fi.HasChilds;
            e.SetNodeValue("ID", fi.ID);
            e.SetNodeValue("Name", fi.Name);
            e.SetNodeValue("FullName", fi.FullName);
            e.SetNodeValue("ParentID", fi.ParentID);
            e.SetNodeValue("Level", fi.Level);
            e.SetNodeValue("Images", fi.ImgPaths);
            e.SetNodeValue("Permissions", fi.Permissions);
            e.SetNodeValue("IsExportContact", fi.IsExportContact);
            e.SetNodeValue("ShowContacts", fi.ShowContacts);
            e.SetNodeValue("ShowNotifications", fi.ShowNotifications);
        }

        public string CreateNodeTemplate(object dataItem) {
            FolderInfo fi = dataItem as FolderInfo;
            if(fi == null)
                return "";

            int level = fi.FullName.Split('\\').Length - 3;

            string content =
                    "<div class='fp_textTreeListNode " + "' style='margin-top:3px;margin-bottom: 3px;' data-id=" +
                    ID + " data-rowId=" + fi.ID + " data-parentId=" + fi.ParentID + " data-path='" + fi.FullName + "' data-name='" + Name + "' data-level='" + level + "' data-saveSelectedKey='"+ SaveSelectedKey+ "' >";
            content += fi.Name;
            var imgPath = "/Areas/Flexpage/Content/Images/frontend/browser/{0}.png";
            foreach(var img in fi.ImgPaths)
            {
                content += "<img class='fp_textTreeListNodeImg' src='" + string.Format(imgPath, img) + "'/> ";
            }

            return content + "</div>";
        }

        private FolderInfo getRootFolder(int childrenCount)
        {
            // check if the root folder exists in the DB
            var filterBy = new List<FilterDesciption>() {
                    new FilterDesciption()
                    {
                        Table = "Folder",
                        Field = "Name",
                        Value = "\\",
                        FilterMode = eFilterMode.Equal
                    }
                };
            var folders = new List<FolderInfo>();
            List<ObjectInfo> result = Repository.QueryFolders.NewQuery().FilterBy(filterBy).ObjectsList(true);
            if(result.Count > 0)
            {
                result[0].Properties.First(p => p.Name == "Folder.Name").Value = "\\Root\\";
                TransformToFolderInfo(result, folders, "\\", 0);
                folders[0].FullName = "\\";
                return folders[0];
            }

            // otherwise return a default root folder
            return new FolderInfo() { Name = "Root", FullName = "\\", HasChilds = childrenCount > 0, Level = -1, ID = 0, ImgPaths = new List<string>() } ;
        }
    }
}