using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;

namespace Flexpage.Models
{
    public class BlockStyleModel : ViewModel
    {
        public BlockStyleModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {

        }
        public int BlockID { get; set; }
        public string CustomCssClass { get; set; }
        public string CustomCss { get; set; }
        public string CustomView { get; set; }
        public List<string> AvailableBlockViews { get; set; }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);

            BlockID = proto.ID;
            Dictionary<string, string> styles = repository.GetBlockStyles(BlockID);
            CustomCssClass = styles["cssClass"];
            CustomView = styles["view"];
            CustomCss = styles["css"];
            AvailableBlockViews = getViewsForBlock(proto.BlockType);

        }

        private List<string> getViewsForBlock(string blockType)
        {
            List<string> viewList = new List<string>();
            string path = _settings.MapPath("~/Areas/Flexpage/Views/Flexpage/CustomViews");
            viewList.Add(String.Empty);
            var allFilesFromFolder = System.IO.Directory.Exists(path) ?
                System.IO.Directory.GetFiles(path, blockType + "*.cshtml")
                .Select(u =>
                {
                    var fileName = Path.GetFileName(u);
                    return fileName.Remove(fileName.Length - ".cshtml".Length);
                })
            : new string[0];
            viewList.AddRange(allFilesFromFolder);
            return viewList;

        }
    }
}