using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Flexpage.Code.StructureManagement
{
    public class TreePlace
    {
        public TreeNode Parent { get; set; }
        public TreeNode Node { get; set; }
        public int Index { get; set; }
    }

    [DataContract]
    public class TreeNode
    {
        [DataMember(Name = "titles")]
        private Dictionary<string, string> Titles { get; set; }

        [DataMember(Name = "pageRefernces")]
        private Dictionary<string, string> PagesReferences { get; set; }


        [DataMember(Name = "isreferences ")]
        private Dictionary<bool, bool> IsReferences { get; set; }

        [DataMember(Name = "descriptions")]
        private Dictionary<string, string> Descriptions { get; set; }

        private string _Title = null;
        [XmlAttribute("title")]
        [DataMember(Name = "title")]
        public string Title
        {
            get { return string.IsNullOrEmpty(_Title) ? null : _Title; }
            set { _Title = value; }
        }

        private string _ReferencePageURl = null;
        [XmlAttribute("referencepageurl")]
        [DataMember(Name = "referencepageurl")]
        public string ReferencePageURl
        {
            get { return string.IsNullOrEmpty(_ReferencePageURl) ? null : _ReferencePageURl; }
            set { _ReferencePageURl = value; }
        }

        private string _PageReference = null;
        [XmlAttribute("pageRefernce")]
        [DataMember(Name = "pageRefernce")]
        public string PageReference
        {
            get { return string.IsNullOrEmpty(_PageReference) ? null : _PageReference; }
            set { _PageReference = value; }
        }

        private bool _IsReference = false;
        [XmlAttribute("isreference")]
        [DataMember(Name = "isreference")]
        public bool IsReference
        {
            get { return _IsReference; }
            set { _IsReference = value; }
        }


        [XmlAttribute("resourceKey")]
        [DataMember(Name = "key")]
        public string Key { get; set; }

        private string _Description = null;
        [XmlAttribute("description")]
        [DataMember(Name = "description")]
        public string Description
        {
            get { return string.IsNullOrEmpty(_Description) ? null : _Description; }
            set { _Description = value; }
        }

        [IgnoreDataMember]
        [System.Web.Script.Serialization.ScriptIgnore]
        [XmlIgnore]
        public Dictionary<string, string> AllTitles
        {
            get
            {
                return Titles;
            }
        }

        [IgnoreDataMember]
        [System.Web.Script.Serialization.ScriptIgnore]
        [XmlIgnore]
        public Dictionary<string, string> AllDescriptions
        {
            get
            {
                return Descriptions;
            }
        }

        public string GetLocalisedTitle(string currentOrDefaultLangCode, string defaultLangCode)
        {
            string title = String.Empty;
            if (this.Titles != null && this.Titles.Count > 0)
            {
                title = this.Titles[currentOrDefaultLangCode];
                if (String.IsNullOrEmpty(title))
                {
                    title = this.Titles[defaultLangCode];
                }
            }
            return title;
        }

        public string GetLocalisedDescription(string currentOrDefaultLangCode, string defaultLangCode)
        {
            string title = String.Empty;
            if (this.Descriptions != null)
            {
                title = this.Descriptions[currentOrDefaultLangCode];
                if (String.IsNullOrEmpty(title))
                {
                    title = this.Descriptions[defaultLangCode];
                }
            }
            return title;
        }

        [XmlIgnore]
        public bool bVisible = true;
        [XmlAttribute("visible")]
        [DataMember(Name = "visible")]
        public bool Visible
        {
            get { return bVisible ? bVisible : false; }
            set { bVisible = false != value; }
        }

        [XmlIgnore]
        public bool bReadonly;
        [XmlAttribute("readonly")]
        [DataMember(Name = "readonly")]
        public string Readonly
        {
            get { return bReadonly ? "true" : null; }
            set { bReadonly = "true" == value; }
        }

        [XmlIgnore]
        public string sCms;
        [XmlAttribute("cms")]
        [DataMember(Name = "cms")]
        public string Cms
        {
            get { return sCms; }
            set { sCms = value; }
        }

        private string _Url = null;
        [XmlAttribute("url")]
        [DataMember(Name = "url")]
        public string Url
        {
            get { return string.IsNullOrEmpty(_Url) ? null : _Url; }
            set { _Url = value; }
        }

        private string _Target = null;
        [XmlAttribute("target")]
        [DataMember(Name = "target")]
        public string Target
        {
            get { return string.IsNullOrEmpty(_Target) ? null : _Target; }
            set { _Target = value; }
        }

        private bool _IsAnonymousOnly = false;
        [XmlAttribute("IsAnonymousOnly")]
        [DataMember(Name = "IsAnonymousOnly")]
        public bool IsAnonymousOnly
        {
            get { return _IsAnonymousOnly; }
            set { _IsAnonymousOnly = value; }
        }

        private string _Roles = null;
        [XmlAttribute("roles")]
        [DataMember(Name = "roles")]
        public string Roles
        {
            get { return string.IsNullOrEmpty(_Roles) ? null : _Roles; }
            set { _Roles = value; }
        }

        private string _VisibleRoles = null;
        [XmlAttribute("visibleRoles")]
        [DataMember(Name = "visibleRoles")]
        public string VisibleRoles
        {
            get { return string.IsNullOrEmpty(_VisibleRoles) ? null : _VisibleRoles; }
            set { _VisibleRoles = value; }
        }

        private string _Languages = null;
        [XmlAttribute("languages")]
        [DataMember(Name = "languages")]
        public string Languages
        {
            get { return string.IsNullOrEmpty(_Languages) ? null : _Languages; }
            set { _Languages = value; }
        }

        [XmlIgnore]
        protected List<TreeNode> _nodes = new List<TreeNode>();

        // sub-nodes are in the parent node directly without <items> wrapper
        // if you need wrapper see below
        [XmlElement("siteMapNode")]
        [DataMember(Name = "nodes")]
        public TreeNode[] Nodes
        {
            get
            {
                var items = new TreeNode[_nodes.Count];
                _nodes.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                var items = (TreeNode[])value;
                _nodes.Clear();
                foreach (var item in items)
                    _nodes.Add(item);
            }
        }

        public TreeNode()
        {
            bVisible = true;
            Titles = new Dictionary<string, string>();
            Descriptions = new Dictionary<string, string>();
        }


        public void LoadResources(IEnumerable<string> languages, Dictionary<string, Dictionary<string, string>> res)
        {
            Titles = new Dictionary<string, string>();
            Descriptions = new Dictionary<string, string>();
            foreach (var lang in languages)
            {
                if (res.ContainsKey(lang))
                {
                    var keyTitle = (Key + ".Title").ToLower();
                    Titles.Add(lang, res[lang].ContainsKey(keyTitle) ? res[lang][keyTitle] : "");
                    var keyDescription = (Key + ".Description").ToLower();
                    Descriptions.Add(lang, res[lang].ContainsKey(keyDescription) ? res[lang][keyDescription] : "");
                }
            }
            foreach (var node in _nodes)
            {
                node.LoadResources(languages, res);
            }
        }

        public void _RemoveNode(TreeNode node)
        {
            _nodes.Remove(node);
        }
        public void _InsertNode(TreeNode node, int index)
        {
            _nodes.Insert(index, node);
        }
        public void _AddNode(TreeNode node)
        {
            _nodes.Add( node);
        }
        internal TreePlace _FindNode(string key)
        {
            for (var i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                if (string.IsNullOrEmpty(node.Key))
                    continue;
                if (node.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return new TreePlace() { Node = node, Parent = this, Index = i };
            }
            foreach (var node in _nodes)
            {
                var place = node._FindNode(key);
                if (null != place)
                    return place;
            }
            return null;
        }

        public void GetAllResourceKeys(List<string> keys)
        {
            keys.Add(Key);
            foreach (var node in _nodes)
            {
                node.GetAllResourceKeys(keys);
            }
        }

        // add unique key for nodes without key
        public bool UpdateKeys(TreeNode root)
        {
            var res = false;
            if (string.IsNullOrEmpty(Key))
            {
                GenerateKey(root);
                res = true;
            }
            foreach (var node in _nodes)
            {
                res |= node.UpdateKeys(root);
            }
            return res;
        }

        // Title should be set-up
        public void GenerateKey(TreeNode root)
        {
            var key = Title;
            if (string.IsNullOrEmpty(key))
                key = "undef";
            key = Regex.Replace(key, @"[^A-Za-z0-9]", "");
            if (Char.IsDigit(key[0]))
                key = "r" + key;
            var keyBase = key;
            for (var i = 1; root.Key == key || null != root._FindNode(key); ++i)
            {
                key = keyBase + i;
            }
            Key = key;
        }

        /// <summary>
        /// Fixes roles (replace ',' with ';')
        /// </summary>
        public void FixRoles()
        {
            // fix Roles
            if ((!string.IsNullOrEmpty(this.Roles)) &&
                (this.Roles.IndexOf(',') > 0))
            {
                this.Roles = this.Roles.Replace(',', ';');
            }

            // fix VisibleRoles
            if ((!string.IsNullOrEmpty(this.VisibleRoles)) &&
                (this.VisibleRoles.IndexOf(',') > 0))
            {
                this.VisibleRoles = this.VisibleRoles.Replace(',', ';');
            }

            // do the same for child nodes
            if ((this.Nodes != null) && (this.Nodes.Length > 0))
            {
                foreach (TreeNode child in this.Nodes)
                {
                    child.FixRoles();
                }
            }
        }

        /// <summary>
        /// Gets roles in list from string with separators
        /// </summary>
        public static List<string> GetRolesFromString(string roles)
        {
            if (!string.IsNullOrEmpty(roles))
            {
                return roles.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return new List<string>();
        }
    }
}