using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web.Mvc;

namespace Flexpage.Models
{
    public class TabModel : BlockListModel
    {
        public TabModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public static TabModel Default(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage)
        {
            return new TabModel(settings, flexpage)
            {
                Alias = NewAlias,
                CssClass = "",
                ID = -1,
                Title = new LocalizedStringModel("{en: \"Tab\"}", settings, flexpage),
                Order = 0
            };
        }

        /// <summary>
        /// Service field for column number display 
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Tab order determines its position in tab container
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Internal use
        /// </summary>
        public bool Last { get; set; }
        public int TabContainerID { get; set; }
        public bool ShowTabToUsers { get; set; }
        public bool IsDefault { get; set; }
        public string UID { get; set; }
        public LocalizedStringModel Title { get; set; }
        public string BackColor { get; set; }
        public TextPosition Position { get; set; } = TextPosition.TopLeft;
        public string HTMLLink { get; set; }
        public bool Open { get; set; }

        public override void Update()
        {
            base.Update();
            Title.Update();
        }

        public void GenerateHTMLLink(string pageUrl)
        {
            HTMLLink = $"<a href=\"{pageUrl}?ID={UID}\" class=\"fpActivateTab\" onclick=\"fp_ActivateTab('{UID}', event);\">{Title.Current}</a>";
        }

        public void Assign(TabModel source)
        {
            base.Assign(source);
            this.Order = source.Order;
            this.Title = source.Title;
            this.ShowTabToUsers = source.ShowTabToUsers;
            this.BackColor = source.BackColor;
            this.Position = source.Position;
            this.IsDefault = source.IsDefault;
            this.UID = source.UID;

            GenerateHTMLLink("#");
        }

        public override void Assign(object source, params object[] args)
        {
            var tp = source as TabPage;
            if (tp != null)
            {
                base.Assign(tp.BlockList, args);
                this.TabContainerID = tp.TabContainerID;
                //2DO
                this.ShowTabToUsers = tp.ShowTabToUsers;
                this.Title = new LocalizedStringModel(tp.BlockList.Name, _settings, _flexpageProcessor);
                this.IsDefault = tp.IsDefault;
                this.Order = tp.BlockList.OrdNum;
                this.UID = tp.UID;
                GenerateHTMLLink("#");
            }
        }


        public void Apply(TabPage target)
        {
            // where Title и Order?
            target.BlockList.Name = (string)this.Title;
            target.BlockList.OrdNum = this.Order;
            target.IsDefault = this.IsDefault;
            // 2DO:
            target.ShowTabToUsers = this.ShowTabToUsers;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {

            base.Apply(repository, args);
            var c = repository.GetByBlockListID<TabPage>(this.ID);
            var tc = args[0] as TabContainer;
            if (c == null && tc != null)
            {
                c = repository.AddTabPage(tc, (string)this.Title, IsDefault, false, Guid.NewGuid().ToString());
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

    }

}