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
    public class CMSFileSelectorModel : ViewModel
    {
        public BrowserModel Browser { get; set; }
        public CMSFileSelectorModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Browser = new BrowserModel(settings, flexpage, true);
        }
        
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            Browser.Load(repository, "pw_browserLinkForCMS", needToLoadContent,true);
        }

        public override void Update()
        {
           
        }
        public void Assign(Maintenance source)
        {
           
        }

        public override void FillViewData(ViewDataDictionary viewData, IFlexpageRepository repository, string Title = "")
        {
            Browser.FolderContentModel.FilterCustomProperties = String.Empty;
            viewData["FilterCustomProperties"] = String.Empty;
            Browser.FolderContentModel.FilterExtension = String.Empty;
            viewData["FilterExtension"] = String.Empty;
            viewData["Title"] = Title;
        }
    }
}