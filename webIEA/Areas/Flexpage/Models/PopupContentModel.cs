using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class PopupContentModel : ViewModel
    {
        public PopupContentModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage) { }
        public string IDPostfix { get; set; }
        public string Command { get; set; }
        public int BlocklistID { get; set; } = 0;
        public string BlockType { get; set; }
        public string BlockAlias { get; set; }
        public string Parameters { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Url { get; set; }
    }
}