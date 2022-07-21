using Flexpage.Abstract;
using Flexpage.Code.StructureManagement;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Helpers;
using Flexpage.Helpers.StructureManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Flexpage.Models
{

    public class SitemapEntryModel
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public int Number { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public string URL { get; set; }
        public string Target { get; set; }
        public Dictionary<string, bool> Languages { get; set; }
        public Dictionary<string, bool> VisibleRoles { get; set; }
        public Dictionary<string, bool> AccessRoles { get; set; }
        public bool Visible { get; set; }
        public bool Readonly { get; set; }
        public bool OnlyForAnonymous { get; set; }
        public bool HasChildren { get; set; }
        public int Index { get; set; }
        public string DefaultTitle { get; set; }
        
        public const string NewEntryID = "NEW_ENTRY";

        public SitemapEntryModel()
        {
            Description = new Dictionary<string, string>();
            Title = new Dictionary<string, string>();
            DefaultTitle = "";
            Target = "";
            URL = "";
            Visible = true;
            OnlyForAnonymous = false;
            Readonly = false;
            Languages = new Dictionary<string, bool>();
            AccessRoles = new Dictionary<string, bool>();
            VisibleRoles = new Dictionary<string, bool>();
            HasChildren = false;
        }

        public SiteMapManager2Model SiteMapManager { get; set; }
    }
    
    public class SitemapNewEntryModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }
        public string PageUrl { get; set; }
        public string ManualURL { get; set; }
        public bool CustomURL { get; set; }
    }

    public class SiteMapManager2Model : ViewModel
    {
        public SiteMapManager2Model(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            _sitemapHelper = new SitemapHelper(settings);
        }
        private readonly SitemapHelper _sitemapHelper;
        public List<SitemapEntryModel> Nodes { get; set; }
        public string SelectedSiteMap { get; set; }
        public List<string> AvailableSitemaps { get; set; }
        public List<string> LoadedSitemaps { get; set; }
        public List<string> AvailablePages { get; set; }
        public SitemapNewEntryModel NewEntry { get; set; }

        public IEnumerable<SitemapEntryModel> GetSelectedSitemapNodes()
        {
            return GetSubnodes(SelectedSiteMap);
        }

        public IEnumerable<SitemapEntryModel> GetSubnodes(string parentID)
        {
            return Nodes.Where(n => n.ParentID == parentID).ToList();
        }

        public IEnumerable<KeyValuePair<string, string>> GetAvailableParentNodes()
        {
            var nodes = new List<KeyValuePair<string, string>>();
            nodes.Add(new KeyValuePair<string, string>("", "(Root)"));
            getAvailableNodes(SelectedSiteMap, nodes);
            return nodes;
        }

        private void getAvailableNodes(string parentID, List<KeyValuePair<string, string>> nodes)
        {
            foreach(SitemapEntryModel node in Nodes.Where(n => n.ParentID == parentID))
            {
                nodes.Add(new KeyValuePair<string, string>(node.ID, node.DefaultTitle));
                getAvailableNodes(node.ID, nodes);
            }
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            Nodes = new List<SitemapEntryModel>();
            AvailableSitemaps = new List<string>(_sitemapHelper.GetAllSiteMaps());
            LoadedSitemaps = new List<string>();
            loadSitemapNodes(repository, SitemapHelper.DefaultSitemapName);
            initAdditionalData(repository);
            NewEntry = new SitemapNewEntryModel();
        }

        public void LoadSitemap(IFlexpageRepository repository, string sitemapName)
        {
            loadSitemapNodes(repository, sitemapName);
            initAdditionalData(repository);
        }

        public void AddSitemap(IFlexpageRepository repository, string sitemapName)
        {
            this.SelectedSiteMap = sitemapName;
            if (!this.LoadedSitemaps.Contains(sitemapName))
            {
                this.LoadedSitemaps.Add(sitemapName);
            }
            if (!this.AvailableSitemaps.Contains(sitemapName))
            {
                this.AvailableSitemaps.Add(sitemapName);
            }
            initAdditionalData(repository);
        }

        public void CancelEdit(IFlexpageRepository repository, string nodeID)
        {
            TreeNode node = null;
            var tree = new TreeList();
            tree = _sitemapHelper.GetSitmapTree(SelectedSiteMap,false);
            node = searchTreeNode(tree.Root, getKeyFromID(nodeID));
            SitemapEntryModel sitemapNode = Nodes.FirstOrDefault(n => n.ID == nodeID);
            if (node != null && sitemapNode != null)
            {
                copyFromNode(node, sitemapNode);
            }
            initAdditionalData(repository);
        }

        public void DeleteNode(IFlexpageRepository repository, string nodeID)
        {
            Nodes.RemoveAll(p => p.ID == nodeID);
            initAdditionalData(repository);
        }

        private string getKeyFromID(string nodeID)
        {
            int idx = nodeID.IndexOf('_');
            string key = nodeID.Substring(idx + 1);
            return key;
        }

        private string getPrefixForID(string sitemapName)
        {
            // we should make node IDs unique between sitemaps
            // use sitemap name as prefix 
            string idPrefix = sitemapName;
            idPrefix = idPrefix.Replace("_", "");
            idPrefix += "_";
            return idPrefix;
        }

        public void AddNewNode(IFlexpageRepository repository, IFlexpageSettings settings)
        {
            var entry = new SitemapEntryModel()
            {
                ID = generateUniqueID(NewEntry.Title),
                ParentID = string.IsNullOrEmpty(NewEntry.Parent) ? SelectedSiteMap : NewEntry.Parent,
                URL = NewEntry.CustomURL ? NewEntry.ManualURL : NewEntry.PageUrl
                //URL = string.IsNullOrWhiteSpace(NewEntry.PageUrl) ? NewEntry.ManualURL : NewEntry.PageUrl
            };

            settings.AllowedLanguages.ToList().ForEach(a => 
            {
                entry.Languages[a] = false;
                entry.Title[a] = entry.Description[a] = "";
            });
            settings.UserRoles.ToList().ForEach(a => 
            {
                entry.AccessRoles[a] = entry.VisibleRoles[a] = false;
            });

            string defaultLanguage = settings.DefaultLangCode;
            entry.Title[defaultLanguage] = NewEntry.Title;
            entry.Description[defaultLanguage] = NewEntry.Description;

            entry.DefaultTitle = getDefaultString(entry.Title);

            Nodes.Add(entry);
            if(!string.IsNullOrWhiteSpace(NewEntry.Parent))
            {
                SitemapEntryModel parent = Nodes.FirstOrDefault(n => n.ID == NewEntry.Parent);
                if(parent != null)
                {
                    parent.HasChildren = true;
                }
            }

            NewEntry = new SitemapNewEntryModel();
            initAdditionalData(repository);
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);
            foreach (string sitemapName in LoadedSitemaps)
            {
               var tree = new TreeList();
                tree.Root = new TreeNode()
                {
                    Key = "undef",
                    Url = "~/",
                    IsReference = false,
                    IsAnonymousOnly = false
                };
                populateSitemapTree(tree, sitemapName, null);
                string sitemapPath = _sitemapHelper.GetSiteMapPath(sitemapName);
                tree.Save(sitemapPath, _settings);
                var resourceEditor = new SiteMapResourceEditor(Path.GetFileName(sitemapPath), _settings);
                resourceEditor.Save(tree);
            }
            return null;
        }

        private void loadSitemapNodes(IFlexpageRepository repository, string sitemapName)
        {
            this.SelectedSiteMap = sitemapName;
            if (!this.LoadedSitemaps.Contains(sitemapName))
            {
                this.LoadedSitemaps.Add(sitemapName);
                var tree = _sitemapHelper.GetSitmapTree(sitemapName);
                createSitemapEntries(this.Nodes, getPrefixForID(sitemapName), sitemapName, tree.Root);
            }
        }

        private void initAdditionalData(IFlexpageRepository repository)
        {
            AvailablePages = new List<string>();
            repository.GetList<Page>().ForEach(p => { AvailablePages.Add(p.PageName); });
            this.Nodes.ForEach(n => n.SiteMapManager = this);
        }

        private void createSitemapEntries(List<SitemapEntryModel> entries, string idPrefix, string parentID, TreeNode parentNode)
        {
            int index = 0;
            foreach (Code.StructureManagement.TreeNode node in parentNode.Nodes)
            {
                SitemapEntryModel entry = createFromNode(node, idPrefix + node.Key, parentID, index++, _settings);
                entries.Add(entry);
                createSitemapEntries(entries, idPrefix, entry.ID, node);
            }
        }

        private void populateSitemapTree(TreeList tree, string parentEntryID, string parentNodeKey)
        {
            foreach (SitemapEntryModel entry in this.Nodes.Where(e => e.ParentID == parentEntryID).OrderBy(e => e.Index))
            {
                string key = tree.AddNode(createTreeNode(getKeyFromID(entry.ID), entry), parentNodeKey, -1);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    populateSitemapTree(tree, entry.ID, key);
                }
            }
        }

        private string generateUniqueID(string title)
        {
            // generate right resource key
            var key = title;
            if (string.IsNullOrEmpty(key))
            {
                key = "newNode";
            }
            key = Regex.Replace(key, @"[^A-Za-z0-9]", "");
            if (Char.IsDigit(key[0]))
            {
                key = "r" + key;
            }
            // generate unique ID 
            string keyBase = key = getPrefixForID(this.SelectedSiteMap) + key;
            for (var i = 1; searchKey(SelectedSiteMap, key); ++i)
            {
                key = keyBase + i;
            }
            return key;
        }

        private bool searchKey(string parentID, string searchID)
        {
            foreach(SitemapEntryModel node in Nodes.Where(n => n.ParentID == parentID))
            {
                if(node.ID == searchID || searchKey(node.ID, searchID))
                {
                    return true;
                }
            }
            return false;
        }

        private TreeNode searchTreeNode(TreeNode parent, string key)
        {
            foreach(TreeNode node in parent.Nodes)
            {
                if(node.Key == key)
                {
                    return node;
                }
                TreeNode child = searchTreeNode(node, key);
                if(child != null)
                {
                    return child;
                }
            }
            return null;
        }

        private TreeNode createTreeNode(string key, SitemapEntryModel fromEntry)
        {
            string defaultLanguage = _settings.DefaultLangCode;
            TreeNode node = new TreeNode()
            {
                Key = key,
                //ParentID = parentID,
                //Description = new Dictionary<string, string>(),
                //Title = new Dictionary<string, string>(),
                Target = fromEntry.Target,
                Url = fromEntry.URL,
                Visible = fromEntry.Visible,
                IsAnonymousOnly = fromEntry.OnlyForAnonymous,
                Readonly = fromEntry.Readonly.ToString(),
                Languages = string.Join(";", fromEntry.Languages.Where(l => l.Value).Select(l => l.Key)),
                Roles = string.Join(";", fromEntry.AccessRoles.Where(l => l.Value).Select(l => l.Key)),
                VisibleRoles = string.Join(";", fromEntry.VisibleRoles.Where(l => l.Value).Select(l => l.Key)),
                Title = fromEntry.Title.ContainsKey(defaultLanguage) ? fromEntry.Title[defaultLanguage] : "",
                Description = fromEntry.Description.ContainsKey(defaultLanguage) ? fromEntry.Description[defaultLanguage] : ""
            };

            fromEntry.Title.ToList().ForEach(t => { node.AllTitles[t.Key] = t.Value; });
            fromEntry.Description.ToList().ForEach(t => { node.AllDescriptions[t.Key] = t.Value; });
            return node;
        }

        private SitemapEntryModel createFromNode(Code.StructureManagement.TreeNode node, string nodeID, string parentID, int index, IFlexpageSettings settings)
        {
            SitemapEntryModel entry = new SitemapEntryModel()
            {
                ID = nodeID,
                ParentID = parentID,
                Index = index
            };

            return copyFromNode(node, entry);
        }

        private SitemapEntryModel copyFromNode(Code.StructureManagement.TreeNode node, SitemapEntryModel copyTo)
        {
            copyTo.Target = node.Target;
            copyTo.URL = node.Url;
            copyTo.Visible = node.Visible;
            copyTo.OnlyForAnonymous = node.IsAnonymousOnly;
            copyTo.Readonly = bool.Parse(node.Readonly ?? "false");
            copyTo.HasChildren = node.Nodes.Length > 0;

            _settings.AllowedLanguages.ToList().ForEach(a => { copyTo.Languages[a] = false; });
            _settings.UserRoles.ToList().ForEach(a => { copyTo.AccessRoles[a] = copyTo.VisibleRoles[a] = false; });

            if (node.Languages != null)
            {
                Array.ForEach<string>(node.Languages.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries), a => { copyTo.Languages[a] = true; });
            }
            if (node.Roles != null)
            {
                Array.ForEach<string>(node.Roles.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries), a => { copyTo.AccessRoles[a] = true; });
            }
            if (node.VisibleRoles != null)
            {
                Array.ForEach<string>(node.VisibleRoles.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries), a => { copyTo.VisibleRoles[a] = true; });
            }

            node.AllTitles.ToList().ForEach(t => { copyTo.Title[t.Key] = t.Value; });
            node.AllDescriptions.ToList().ForEach(t => { copyTo.Description[t.Key] = t.Value; });
            copyTo.DefaultTitle = getDefaultString(copyTo.Title);

            return copyTo;
        }

        private string getDefaultString(Dictionary<string, string> dict)
        {
            string desc = "";
            dict.TryGetValue(_settings.DefaultLangCode, out desc);
            return desc;
        }
    }
}