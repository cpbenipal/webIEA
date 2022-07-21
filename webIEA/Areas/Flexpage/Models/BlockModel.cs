using System;
using System.Linq;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Abstract.DTO;

namespace Flexpage.Models
{
    public class BlockModel: ViewModel, IBlockModel
    {
        public const int NewStaticBlockID = -1;

        public string ErrorsLog { get; set; } = "";

        public int BlocklistID { get; set; }

        public string Alias { get; set; } = "";
        public bool Visible { get; set; } = false;
        public string CssClass { get; set; } = "";
        public string Css { get; set; } = "";
        public string CustomView { get; set; } = "";
        public bool IsCopyable { get; set; } = false;
        public int BlockAfter { get; set; } = 0;
        public bool HasView { get; set; }

        public string FrontViewPath { get; set; }
        public string EditorViewPath { get; set; }

        private string _BlockType;
        public string BlockType {
            get
            {
                string postfix = "Model";
                if(_BlockType == null)
                    _BlockType = this.GetType().Name;
                return _BlockType.EndsWith(postfix) ? _BlockType.Remove(_BlockType.Length - postfix.Length) : _BlockType;
            }
            private set { _BlockType = value; }
        }

        public BlockModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) : base(settings, flexpageProcessor)
        {
        }

        public BlockModel(string type, string css, bool hasView, Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor)
            : this(settings, flexpageProcessor)
        {
            CssClass = css;
            BlockType = ExtractShortTypeName(type);
            HasView = hasView;
            ///2do: remove after test finished
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            BlocklistID = proto.BlocklistID;
            Alias = proto.BlockAlias;
            calculateBlockView();
        }

        public override void Assign(object source, params object[] args)
        {
            base.Assign(source, args);

            if(source is Block)
            {
                Block block = source as Block;
                Visible = block.Visible;
                if(_flexpageProcessor != null) //Some blocks can create a blockmodel without link to the IFlexpage. 
                {
                    Visible = _flexpageProcessor.SetBlockVisibility(block.ID, block.Visible);
                }
                Alias = block.Alias;
                ID = block.ID;

                _BlockType = block.BlockType.Name;

                CssClass = block.CssClass;
                Css = block.Css;
                if (!String.IsNullOrEmpty(block.CustomView) && System.IO.File.Exists(_settings.MapPath(String.Format("~/Areas/Flexpage/Views/Flexpage/CustomViews/{0}.cshtml", block.CustomView))))
                {
                    CustomView = block.CustomView;
                }

                if (block is ICopyable)
                {
                    IsCopyable = true;
                }
            }
            calculateBlockView();
        }

        protected virtual void apply(Block block)
        {
            if (block != null)
            {
                block.Alias = Alias;
                block.CssClass = CssClass;
                block.Css = Css;
            }
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);

            if (args != null && args.Count() > 0)
            {
                Block block = args[0] as Block;
                if(block != null)
                {
                    block.Alias = Alias;
                    block.CssClass = CssClass;
                    block.Css = Css;
                    //block.Visible = Visible;

                    return block;
                }
            }
            return null;
        }

        private void calculateBlockView()
        {
            string blockTypeName = (this.BlockType ?? this.GetType().Name).Replace("Model", "");
            if (string.IsNullOrEmpty(this.FrontViewPath))
            {
                if (String.IsNullOrWhiteSpace(this.CustomView))
                {
                    this.FrontViewPath = String.Format("~/Areas/Flexpage/Views/Flexpage/{0}.cshtml", blockTypeName);
                }
                else
                {
                    this.FrontViewPath = String.Format("~/Areas/Flexpage/Views/Flexpage/CustomViews/{0}.cshtml", this.CustomView);
                }
            }
            if (string.IsNullOrEmpty(this.EditorViewPath))
            {
                this.EditorViewPath = String.Format("~/Areas/Flexpage/Views/Flexpage/Editors/{0}.cshtml", blockTypeName);
            }
        }
    }
}