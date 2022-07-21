using Flexpage.Code.StructureManagement;
using Flexpage.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Flexpage.Domain.Entities;
using Flexpage.Helpers.StructureManagement;
using System.ComponentModel.DataAnnotations;

namespace Flexpage.Models
{
    public class MenuModel : BlockModel
    {
        public MenuModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) : base(settings, flexpageProcessor)
        {
            _sitemapHelper = new SitemapHelper(settings);
        }

        [Required]
        public string Name { get; set; }
        public string SiteMap { get; set; }
        public bool ShowName { get; set; }
        public List<string> SitemapFileNames { get; set; }
        public List<MenuSitemapNode> MenuSitemapNode { get; set; }
        public string SelectedNodes { get; set; }

        //public MenuSettingsModel Settings { get; set; }
        public IEnumerable<MenuItemModel> Items { get; set; }
        public string ClientID { get; set; } //??????
        public int Level { get; set; }

        private static int nodeCounter;
        private readonly SitemapHelper _sitemapHelper;

        /// <summary>
        /// Fills menu model with settings
        /// </summary>
        /// <param name="source">settings</param>
        /// <param name="args">Empty</param>
        public override void Assign(object source, params object[] args)
        {
            Menu s = source as Menu;
            base.Assign(s.Block);
            this.Name = s.Name;
            this.ShowName = s.ShowName;
            this.MenuSitemapNode = s.MenuSitemapNode.ToList();
            this.ClientID = "fp_Menu" + s.BlockID;
            assignSitemapNodes();
        }

        public override void AssignDefaultValues(params object[] args)
        {
            base.AssignDefaultValues(args);
            this.Name = "DefaultMenu";
            this.ShowName = false;
            this.MenuSitemapNode = new List<MenuSitemapNode>();
            this.ClientID = "fp_Menu";
            assignSitemapNodes();
        }

        private void assignSitemapNodes()
        {
            this.SitemapFileNames = _sitemapHelper.GetAllSiteMaps();
            this.SiteMap = this.MenuSitemapNode.Select(msn => msn.SiteMap).FirstOrDefault();
            if (this.SiteMap == null)
            {
                this.SiteMap = SitemapHelper.DefaultSitemapName;
            }
            this.SelectedNodes = String.Join(";", this.MenuSitemapNode.Select(msn => msn.ResourceKey)) + ";";
            CreateMenuPoints(_sitemapHelper.GetSitmapTree(this.SiteMap));
        }


        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            Menu block = null;

            if (proto.ID == BlockModel.NewStaticBlockID && !string.IsNullOrEmpty(proto.BlockAlias))
            {
                block = repository.GetByAlias<Menu>(proto.BlockAlias);
            }
            else
            {
                block = repository.GetByBlockID<Menu>(proto.ID);
            }

            if (block == null)
            {
                AssignDefaultValues();
            }
            else
            {
                Assign(block, proto.BlockAlias);
            }
        }

        private static List<MenuItemModel> CreateMenuItems(TreeNode[] nodes, List<MenuSitemapNode> menuSitemapNode, int level, int parentCounter,
            Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor)
        {
            string curLang = settings.GetCurrentOrDefaultLangCode();
            List<MenuItemModel> menuItems = new List<MenuItemModel>();
            foreach (var node in nodes)
            {
                if (node.Languages == null || (node.Languages != null && node.Languages.Contains(curLang)))
                {
                    var menuItem = new MenuItemModel();
                    var nodeKey = node.Key.Replace("&", "And");

                    string title = node.GetLocalisedTitle(settings.GetCurrentOrDefaultLangCode(), settings.DefaultLangCode);
                    menuItem.Text = String.IsNullOrEmpty(title) ? node.Title : title;
                    menuItem.Url = (node.Url != null) ? node.Url : string.Empty;
                    menuItem.IsVisible = menuSitemapNode != null && menuSitemapNode.Count > 0 && menuSitemapNode.Any(p => p.ResourceKey == nodeKey);
                    menuItem.ResourceKey = node.Key;
                    menuItem.CurrentCounter = nodeCounter;
                    menuItem.ParenCounter = parentCounter;
                    nodeCounter++;
                    if (node.Nodes != null && node.Nodes.Length > 0)
                    {
                        menuItem.SubMenu = CreateSubmenu(node.Nodes, level + 1, menuSitemapNode, menuItem.CurrentCounter, settings, flexpageProcessor);
                    }
                    menuItems.Add(menuItem);
                }
            }
            return menuItems;
        }

        // why static???
        private static MenuModel CreateSubmenu(TreeNode[] nodes, int level, List<MenuSitemapNode> menuSitemapNode, int parentCounter,
            Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor)
        {
            MenuModel menu = new MenuModel(settings, flexpageProcessor);
            menu.Level = level;
            var childs = CreateMenuItems(nodes, menuSitemapNode, level, parentCounter, settings, flexpageProcessor);
            menu.Items = childs;
            return menu;
        }

        private void CreateMenuPoints(TreeList menuTreeList)
        {
            nodeCounter = 0;
            this.Level = 0;
            var childs = CreateMenuItems(menuTreeList.Root.Nodes, this.MenuSitemapNode, this.Level, nodeCounter, _settings, _flexpageProcessor);
            this.Items = childs;
        }

        public override void Update()
        {
            base.Update();
            this.SitemapFileNames = _sitemapHelper.GetAllSiteMaps();
            CreateMenuPoints(_sitemapHelper.GetSitmapTree(this.SiteMap));
        }

        /// <summary>
        /// Apply changes to the DB entity
        /// </summary>
        /// <param name="repository"></param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository);

            Menu menu = repository.GetByBlockID<Menu>(ID);
            if (menu == null)
            {
                if (Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                menu = repository.CreateNewMenu(alias: Alias);
                repository.AddBlockToBlockList(menu.Block, BlocklistID, BlockAfter);
            }

            menu.Name = this.Name;
            menu.ShowName = this.ShowName;
            if (!string.IsNullOrEmpty(this.SelectedNodes))
            {
                List<MenuSitemapNode> toDelete = menu.MenuSitemapNode.ToList();
                foreach (MenuSitemapNode node in toDelete)
                {
                    repository.DeleteEntity<MenuSitemapNode>(node.ID);
                }

                this.SelectedNodes = this.SelectedNodes.Substring(0, this.SelectedNodes.Length - 1);
                string[] nodes = this.SelectedNodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string n in nodes)
                {
                    MenuSitemapNode node = new MenuSitemapNode();
                    node.SiteMap = this.SiteMap;
                    node.MenuID = menu.BlockID;
                    node.ResourceKey = n;
                    menu.MenuSitemapNode.Add(node);
                }
            }
            //repository.ApplyChanges();
            return null;
        }
    }

    public class MenuSettingsModel
    {
        public string TopLevelSeparator { get; set; } = "";
        public string SubmenuLevelSeparator { get; set; } = "";
        public string ShowEffect { get; set; }
        public string HideEffect { get; set; }
        public double? Duration { get; set; }
        public int? TopOffset { get; set; }
        public int? LeftOffset { get; set; }
        public double? AutocloseTimeout { get; set; }
        public bool? PositionSubmenu { get; set; }
        public bool? OneSubmenuVisible { get; set; }
        public bool? MainMenuActive { get; set; }
        public bool? AllowForMobile { get; set; }
    }

    public class MenuItemModel
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public string ResourceKey { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsVisible { get; set; }
        public MenuModel SubMenu { get; set; }
        public int CurrentCounter { get; set; }
        public int ParenCounter { get; set; }
    }
}