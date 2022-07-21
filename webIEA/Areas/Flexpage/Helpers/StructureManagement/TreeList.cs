using Flexpage.Abstract;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Flexpage.Code.StructureManagement
{
    [XmlRoot("siteMap", Namespace = "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0")]
    public class TreeList
    {
        [XmlAttribute("enableLocalization")]
        public string EnableLocalization
        {
            get { return "true"; }
            set { }
        }

        [XmlElement("siteMapNode")]
        public TreeNode Root { get; set; }

        public void Save(string path, IFlexpageSettings settings)
        {
            if(path.First() == '/')
            {
                path = "~" + path;
            }
            if(path.Contains("~/") == true)
            {
                path = settings.MapPath(path);
            }
            var s = new XmlSerializer(typeof(TreeList));
            TextWriter w = new StreamWriter(path);
            s.Serialize(w, this);

            w.Flush();
            w.Close();
        }

        public string AddNode(TreeNode node, string keyTo, int indexTo)
        {
            //node.GenerateKey(Root); // generate unique key by title

            TreeNode nodeTo = null;
            if (string.IsNullOrEmpty(keyTo))
                nodeTo = Root;
            else
            {
                var placeTo = Root._FindNode(keyTo);
                if (null != placeTo)
                    nodeTo = placeTo.Node;
            }

            if (null == nodeTo)
                return null;

            if (indexTo >= 0)
            {
                nodeTo._InsertNode(node, indexTo);
            }
            else
            {
                nodeTo._AddNode(node);
            }
            return node.Key;
        }

        private bool isUrlUnique(TreeNode root, string url, string key)
        {
            if(string.IsNullOrEmpty(url))
                return true;

            bool unique = !root.Nodes.Where(n => !n.Key.Equals(key))
                .Any(n => url.Equals(n.Url, StringComparison.InvariantCultureIgnoreCase));
            for(int i = 0; i < root.Nodes.Length && unique; i++)
                unique = isUrlUnique(root.Nodes[i], url, key);
            return unique;
        }
    }
}