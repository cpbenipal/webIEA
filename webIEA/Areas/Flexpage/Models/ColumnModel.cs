using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web.Mvc;
using Flexpage.Code.CMS;

namespace Flexpage.Models
{
    public class ColumnModel: BlockListModel
    {
        public ColumnModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public static ColumnModel Default(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage)
        {
            return new ColumnModel(settings, flexpage)
            {
                Alias = NewAlias,
                CssClass = "",
                ID = -1,
                Title = new LocalizedStringModel("{en: \"Column\"}", settings, flexpage),
                Order = 0,
                Width = 100,
                CustomWidthUnit = 0
            };
        }

        private static List<SelectListItem> units = new List<SelectListItem>() {
            new SelectListItem() { Text = "px", Value = "0" },
            new SelectListItem() { Text = "%", Value = "1" }
        };

        public IList<SelectListItem> Units
        {
            get
            {
                return units;
            }
        }


        /// <summary>
        /// Service field for column number display 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Service field for column ordering controls display 
        /// </summary>
        public bool Last { get; set; } = false;

        /// <summary>
        /// Service field for toggling headers from parent block
        /// </summary>
        public bool ShowHeader { get; set; }

        /// <summary>
        /// Parent block ID
        /// </summary>
        public int MultiColumnID { get; set; }

        /// <summary>
        /// Width of the column
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 0 - custom width defined by Width prop, 1 - full width, 2 - minimum width
        /// </summary>
        public int WidthType { get; set; }
        /// <summary>
        /// 0 - px, 1 - %
        /// </summary>
        public int CustomWidthUnit { get; set; }
        public int Order { get; set; }
        public LocalizedStringModel Title { get; set; }

        public string HtmlWidth
        {
            get
            {
                switch (WidthType)
                {
                    case 1: return "width = 100%";
                    case 2: return "";
                    default: return String.Format("width = {0}{1}", Width, Units[CustomWidthUnit].Text);
                }
            }
        }

        public override void Update()
        {
            base.Update();
            Title.Update();
        }

        public void Assign(ColumnModel source)
        {
            base.Assign(source);
            this.Width = source.Width;
            this.WidthType = source.WidthType;
            this.CustomWidthUnit = source.CustomWidthUnit;
            this.Order = source.Order;
            this.Title = source.Title;
        }

        public void Apply(Column target)
        {
            // где Title и Order?
            target.WidthUnit = this.CustomWidthUnit;
            target.Width = (int)this.Width;
            target.WidthType = this.WidthType;
            target.BlockList.Name = (string)this.Title;
            target.BlockList.OrdNum = this.Order;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {

            base.Apply(repository, args);
            var c = repository.GetByBlockListID<Column>(this.ID);
            var mc = args[0] as MultiColumn;
            if (c == null && mc != null)
            {
                c = repository.AddColumn(mc, (string)this.Title);
            }
            Apply(c);
            return this;
        }

        public override object AddBlock(BlockModel block)
        {
            return base.AddBlock(block);
        }

        public void SelectLanguage(string langCode)
        {
            Title.SelectLanguage(langCode);
            Update();
        }

        public override void Assign(object source, params object[] args)
        {
            var c = source as Column;
            if (c != null)
            {
                base.Assign(c.BlockList, args);
                this.MultiColumnID = c.MultiColumnID;
                this.CustomWidthUnit = c.WidthUnit;
                this.Width = c.Width;
                this.WidthType = c.WidthType;
                this.Title = new LocalizedStringModel(c.BlockList.Name, _settings, _flexpageProcessor);
                this.Title.SelectLanguage(_settings.GetCurrentOrDefaultLangCode());
                this.Order = c.BlockList.OrdNum;
            }
        }

        public bool IsAdmin()
        {
            return _settings.IsCmsAdmin();
        }
    }

}