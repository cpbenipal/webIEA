using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web.Mvc;

namespace Flexpage.Models
{
    public class FolderSaveContactsSelectorModel : ViewModel
    {
       public FolderTreeListModel FolderTreeListModel { get; set; }
        public FolderSaveContactsSelectorModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            FolderTreeListModel = new FolderTreeListModel(settings, flexpage);
            FolderTreeListModel.PWBrowser = true;

        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            this.Load(repository, proto, title, needToLoadContent, false, "");
        }

        public void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true, bool SaveSelectedNode=false, string alias="")
        {
           alias += "_folderSaveContacts_" ;
            var blockFolderTreeList = repository.GetByAlias<FolderTreeList>("_FolderTreeList" + alias);
            if (blockFolderTreeList == null)
            {
                FolderTreeListModel.AssignDefaultValues();
                FolderTreeListModel.Visible = true;
                FolderTreeListModel.Alias = "_FolderTreeList" + alias;
                FolderTreeListModel.Name = alias;
                FolderTreeListModel.SaveSelectedNode = SaveSelectedNode;
                blockFolderTreeList = FolderTreeListModel.Apply(repository) as FolderTreeList;
                FolderTreeListModel.ID = blockFolderTreeList.BlockID;
            }
            FolderTreeListModel.Load(repository,
                    new BlockCommandModel(_settings) {BlockAlias = alias, ID = blockFolderTreeList.BlockID},"",
                    needToLoadContent);
            FolderTreeListModel.IsStatic = true;
            
        }

        public override void Update()
        {
           
        }
        public void Assign(Maintenance source)
        {
           
        }
        public override void FillViewData(ViewDataDictionary viewData, IFlexpageRepository repository, string Title = "")
        {
            viewData["Title"] = Title;
        }
    }
}