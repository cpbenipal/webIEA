using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class BlockCommandModel : ViewModel
    {
        public string IDPostfix { get; set; }
        public string Command { get; set; }
        public int BlocklistID { get; set; } = 0;
        public string BlockType { get; set; }
        public string BlockAlias { get; set; }
        public string Parameters { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Url { get; set; }
        public bool IsEditor { get; set; } = true;
        public int AllowSave { get; set; } = 1;
        public int BlockAfter { get; set; } = 0;
        public string Title { get; set; }
        public string OneButtonText { get; set; } = "SAVE";

        public BlockCommandModel(Flexpage.Abstract.IFlexpageSettings settings) : base(settings, null)
        {
        }

        public BlockCommandModel(Flexpage.Abstract.IFlexpageSettings settings, Flexpage.Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public static BlockCommandModel Create(BlockModel source, Flexpage.Abstract.IFlexpageSettings settings)
        {
            var cm = new BlockCommandModel(settings, null);
            cm.IDPostfix = source.EditorPostfix;
            cm.Command = "";
            cm.BlocklistID = source.BlocklistID;
            cm.BlockType = source.BlockType;
            cm.BlockAlias = source.Alias;
            cm.Parameters = "";
            cm.Controller = "";
            cm.Action = "";
            cm.Url = "";
            cm.Title = "";
            return cm;
        }
    }
}