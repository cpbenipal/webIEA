using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flexpage.Helpers;

namespace Flexpage.Providers
{
    public class VisibilityXmlSiteMapProvider : XmlSiteMapProvider
    {
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            if (!SecurityTrimmingEnabled)
                return true;

            // check for visibility
            if (null != node["visible"] && "false".Equals(node["visible"].ToLower()))
                return false;

            // check for anonymous
            bool isAnonymousOnly;
            if ((node["IsAnonymousOnly"] != null) &&
                (bool.TryParse(node["IsAnonymousOnly"], out isAnonymousOnly)))
            {
                if (isAnonymousOnly)
                {
                    return !HttpContext.Current.User.Identity.IsAuthenticated;
                }
            }

            // check for roles
            List<string> visibleRoles = Flexpage.Code.StructureManagement.TreeNode.GetRolesFromString(node["visibleRoles"]);
            if ((node.Roles != null && node.Roles.Count > 0 && !node.Roles.Cast<string>().Any(s => context.User.IsInRole(s)))
                ||
                (visibleRoles.Count > 0 && !visibleRoles.Any(s => context.User.IsInRole(s))))
            {
                return false;
            }

            // check for languages
            string languages = node["languages"];
            if (!String.IsNullOrEmpty(languages)
                && !languages.Split(';', ',').Contains(FlexpageSettings.Instance.GetCurrentOrDefaultLangCode()))
                return false;

            return true;
        }

        public override SiteMapNode FindSiteMapNodeFromKey(string key)
        {
            var node = base.FindSiteMapNodeFromKey(key);
            if (node == null)
            {
                var allNodes = this.RootNode.GetAllNodes().Cast<System.Web.SiteMapNode>();
                node = allNodes.FirstOrDefault(u => u.ResourceKey == key);
                if (System.Web.HttpContext.Current != null && node != null)
                {
                    if (IsAccessibleToUser(System.Web.HttpContext.Current, node) == false)
                    {
                        node = null;
                    }
                }
            }
            return node;

        }
    }
}