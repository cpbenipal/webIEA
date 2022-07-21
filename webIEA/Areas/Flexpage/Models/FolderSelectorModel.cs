using DevExpress.Web.ASPxTreeList;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq;

using Pluritech.Shared.Abstract.DTO;
using Pluritech.Shared.Abstract.DTO.Extensions;

namespace Flexpage.Models
{
    public class FolderInfo
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int Level { get; set; }
        public bool HasChilds { get; set; }
        public List<string> ImgPaths { get; set; }
        public int Permissions { get; set; }
        public bool IsExportContact { get; set; }
        public bool ShowContacts { get; set; }
        public bool ShowNotifications { get; set; }
    }

    public class FolderSelectorModel : ViewModel
    {
        public List<FolderInfo> Folders { get; set; }

        private IFlexpageRepository _repository { get; set; }

        public FolderSelectorModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title="", bool needToLoadContent = true)
        {
            _repository = repository;
            /*
            var result = repository.QueryFolders;
            var orderBy = new List<OrderByDesciption>()
            {
                new OrderByDesciption() { Table = "Folder", Field = "Name" }
            };
            result = result.OrderBy(orderBy);
            TransformToFolderInfo(result.ObjectsList(), string.Empty, 0);*/
        }

        private void TransformToFolderInfo(List<ObjectInfo> source, string parentName, int parentID)
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
                    if(parts.Length == 1)
                    {
                        Folders.Add(new FolderInfo()
                        {
                            ID = folder.ID,
                            Name = parts[0],
                            ParentID = parentID,
                            FullName = folderName,
                            Level = Convert.ToInt32(folder.GetPropertyValue("Folder.Level")),
                            HasChilds = Convert.ToBoolean(folder.GetPropertyValue("Folder.HasChilds"))
                        });
                        source.Remove(folder);
                        TransformToFolderInfo(source, folderName, folder.ID);
                    }
                }
                else 
                    if(!string.IsNullOrEmpty(parentName))
                        return;
            }
        }

        public void VirtualModeCreateChildren(TreeListVirtualModeCreateChildrenEventArgs e)
        {
            string parent = e.NodeObject == null ? "\\" : (e.NodeObject as FolderInfo).FullName;
            int level = e.NodeObject == null ? -1 : (e.NodeObject as FolderInfo).Level;

            List<ObjectInfo> children = new List<ObjectInfo>();
            //get child nodes
            var result = _repository.QueryFolders;
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
            Folders = new List<FolderInfo>();
            TransformToFolderInfo(result.ObjectsList(false), parent, 0);
            if (e.NodeObject == null && parent == "\\")
            {
                e.Children = new List<FolderInfo>() { new FolderInfo() { Name = "Root", FullName = "\\", HasChilds = Folders.Count() > 0, Level = -1, ID = 0, ImgPaths = new List<string>() } };
            }
            else
            {
                e.Children = Folders;
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
        }
    }
}