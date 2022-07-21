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
    public class BrowserSelectorModel : BrowserModel
    {
        public string UserName { get; set; }
        public BrowserSelectorModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage,string userName) : base(settings, flexpage,true)
        {
            UserName = userName;
        }
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            FolderTreeListModel.SaveSelectedKey = "SelectNode_AdvancedSearch_" + UserName;
            base.Load(repository, "pw_browserChooseAdvancedSearchVer3", needToLoadContent,true,true);
        }

        
        public override void FillViewData(ViewDataDictionary viewData, IFlexpageRepository repository, string Title = "")
        {
            //this.FolderContentModel.FilterCustomProperties = "ContactsQuery";
            //viewData["FilterCustomProperties"] = this.FolderContentModel.FilterCustomProperties;
            //this.FolderContentModel.FilterExtension = ".xml";
            //viewData["FilterExtension"] = this.FolderContentModel.FilterExtension;
            viewData["Title"] = Title;
            viewData["UserName"] = UserName;
        }
    }
}