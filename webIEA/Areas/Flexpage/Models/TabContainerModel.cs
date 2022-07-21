using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web.Mvc;
using System.Text;
using Pluritech.Contact.Abstract.DTO;


namespace Flexpage.Models
{
    public class TabContainerModel : BlockModel
    {
        public EditorType EditorType { get; set; }
        public int Index { get; set; } = 0;
        public string PageUrl { get; set; }
        public List<TabModel> Tabs { get; set; }
        private List<SelectListItem> tabsDropDown = null;

        public bool SwitchAutomatically { get; set; }

        public int SwitchDelay { get; set; }

        public eTabsSwitchTransition SwitchTransition { get; set; }

        public enum eTabsSwitchTransition
        {
            Default = 0,
            Slideshow = 1
        }

        public IList<SelectListItem> TabsDropDown
        {
            get
            {
                if (tabsDropDown == null)
                {
                    tabsDropDown = new List<SelectListItem>();
                    int i = 0;
                    // foreach (TabModel t in this.Tabs.OrderBy(m => m.Order))
                    foreach (TabModel t in this.Tabs)
                    {
                        tabsDropDown.Add(new SelectListItem() { Selected = i == DefaultTab,
                            Text = t.Title.NotEmptyLocalization, Value = i.ToString() });
                        i++;
                    }
                    if (DefaultTab < 0 && Tabs.Count > 0)
                        Tabs.FirstOrDefault().IsDefault = true;
                }
                return tabsDropDown;
            }
        }

        public bool ShowHeaders { get; set; } = true;
        public int DefaultTab { get; set; } = 0;
        public string HTMLCodeForMenu { get; set; } = "";
        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                return GetLanguageSelector(Tabs.FirstOrDefault().Title.CurrentLangCode,
                    Tabs.Select(t => t.Title).ToList(),
                    new List<LocalizedTextModel>() { },
                    "fp_tabContainer_SelectLanguage");
            }
        }

        public TabContainerModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) :base(settings, flexpageProcessor)
        {
            EditorType = EditorType.Simple;
        }

        public TabContainerModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor, object source):base(settings, flexpageProcessor)
        {
            EditorType = EditorType.Simple;
            Assign(source);
        }

        public void Reorder()
        {
            Tabs = new List<TabModel>(Tabs.OrderBy(e => e.Order));
            UpdateOrder();
        }

        public void UpdateOrder()
        {
            int i = 0;
            foreach (var c in Tabs)
            {
                c.Order = i;
                i++;
            }
        }

        public void ChangeOrder(int index, int delta)
        {
            var c = Tabs[index];
            Tabs.Remove(c);
            var i = index + delta;
            i = i < 0 ? 0 : i >= Tabs.Count ? Tabs.Count : i;
            Tabs.Insert(i, c);
            UpdateOrder();
        }

        public void GenerateMenuCode()
        {
            if (PageUrl == null)
                return;
            StringBuilder objSB = new StringBuilder();

            int i = PageUrl.IndexOf('?');
            string pu;

            if (i > 0)
                pu = PageUrl.Substring(0, i);
            else
                pu = PageUrl;
            objSB.Append("<div>");
            objSB.Append(@"<ul class='nav nav-pills tabs-navigation tabbed-content-nav'>");
            foreach (TabModel t in Tabs)
            {
                objSB.Append("<li>");
                t.GenerateHTMLLink(pu);
                objSB.Append(t.HTMLLink);
                objSB.Append("</li>");
            }
            objSB.Append("</ul>");
            objSB.Append("</div>");
            HTMLCodeForMenu = objSB.ToString();
        }

        public override void Assign(object source, params object[] args)
        {
            if (source is TabContainer tc)
            {
                base.Assign(tc.Block, args);
                SwitchAutomatically = tc.SwitchAutomatically;
                SwitchDelay = tc.SwitchDelay;
                SwitchTransition = (eTabsSwitchTransition)tc.SwitchTransition;
                this.ShowHeaders = tc.IsShowHeader;
                this.Tabs = new List<TabModel>();
                int i = 0;
                foreach (var tp in tc.TabPage)
                {
                    var tm = new TabModel(_settings, _flexpageProcessor);
                    tm.Assign(tp);
                    this.Tabs.Add(tm);
                    if (tp.IsDefault)
                        // this.DefaultTab = tp.BlockList.ID;
                        this.DefaultTab = i;
                }
                Reorder();
            }
        }

        public void Apply(IFlexpageRepository repository, TabContainer target)
        {
            int i = 0;
            target.IsShowHeader = this.ShowHeaders;
            target.SwitchDelay = SwitchDelay;
            target.SwitchAutomatically = SwitchAutomatically;
            target.SwitchTransition = (int)SwitchTransition;
            foreach (TabModel tm in Tabs)
            {
                tm.IsDefault = this.DefaultTab == i;
                tm.Apply(repository, target);
                i++;
            }

            List<TabPage> rl = new List<TabPage>();
            foreach (TabPage tp in target.TabPage)
            {
                if (tp.BlockListID != 0 && !Tabs.Any(e => e.ID == tp.BlockListID))
                    rl.Add(tp);
            }

            foreach (TabPage tp in rl)
                repository.RemoveTabPage(tp);
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository);

            var tc = repository.GetByBlockID<TabContainer>(ID);
            if (tc == null)
            {
                if (Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                tc = repository.CreateNewTabContainer(alias: Alias);
                repository.AddBlockToBlockList(tc.Block, BlocklistID, BlockAfter);
            }
            Apply(repository, tc);

            return null;
        }

        private void AssignDefaultValues(IFlexpageRepository repository)
        {
            TabModel c = TabModel.Default(_settings, _flexpageProcessor);
            this.Alias = repository.CreateNewAlias;
            Tabs = new List<TabModel>();
            Tabs.Add(c);
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            this.Alias = proto.BlockAlias;
            this.PageUrl = proto.Url;
            var tc = repository.GetByBlockID<TabContainer>(proto.ID);
            if (tc == null)
                AssignDefaultValues(repository);
            else
                Assign(tc);
            if (needToLoadContent)
            {
                foreach (TabModel t in Tabs)
                    t.LoadContent(repository);
            }
            GenerateMenuCode();

        }

        /// <summary>
        /// Creates new tab
        /// </summary>
        public void AddTab()
        {
            TabModel t = TabModel.Default(_settings, _flexpageProcessor);
            int mo = 0;
            if (Tabs.Count > 0)
                mo = Tabs.Max(e => e.Order) + 1;
            t.Title = new LocalizedStringModel("{\"en\": \"" + String.Format("Tab {0}", mo) + "\"}", _settings, _flexpageProcessor);
            t.Order = mo;
            // t.ParentAlias = this.Alias;
            // t.ParentType = this.BlockType;
            t.ID = -1;
            t.Alias = NewAlias;
            t.TabContainerID = this.ID;
            Tabs.Add(t);
        }

        /// <summary>
        /// Removes tab from tabs list
        /// </summary>
        /// <param name="index">Zero-bazed tab index</param>
        public void DeleteTab(int index)
        {
            Tabs.RemoveAt(index);
            Reorder();
        }

        public override void Delete(IFlexpageRepository repository)
        {
            base.Delete(repository);
        }

        public override void Update()
        {
            base.Update();
            foreach (var t in Tabs)
            {
                t.Update();
                t.IsDefault = t.ID == this.DefaultTab;
            }
        }

        public void SelectLanguage(string langCode)
        {
            foreach (var c in Tabs)
                c.SelectLanguage(langCode);
        }
    }
}