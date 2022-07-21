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
    public class FolderSaveAsContactsSelectorModel : FolderSaveContactsSelectorModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public FolderSaveAsContactsSelectorModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, string userName) 
            : base(settings, flexpage)
        {
            Name = "contact-advanced-search";
            UserName = userName;
        }

        public new void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true, bool SaveSelectedNode = false, string alias = "ver1")
        {
            base.Load(repository, proto, title, needToLoadContent, SaveSelectedNode, alias);
            FolderTreeListModel.SaveSelectedKey = "SelectNode_AdvancedSearch_" + UserName;
        }

        public override void FillViewData(ViewDataDictionary viewData, IFlexpageRepository repository, string Title = "")
        {
            base.FillViewData(viewData, repository, Title);
        }
    }
}