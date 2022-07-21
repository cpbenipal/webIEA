using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Flexpage.Code.StructureManagement;
using System.Web.Configuration;
using System.Configuration;
using Flexpage.Abstract;
using System.Xml.Serialization;

namespace Flexpage.Helpers.StructureManagement
{
    public class SitemapHelper
    {
        public const string DefaultSitemapName = "MainSiteMap";
        private readonly IFlexpageSettings _settings;
        public SitemapHelper(IFlexpageSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Returns a list of the sitemaps available.
        /// </summary>
        /// <returns>A list of the sitemap names.</returns>
        public List<string> GetAllSiteMaps()
        {
            string path = _settings.MapPath("~/SiteMap");

            List<string> list = new List<string>();
            list.Add(DefaultSitemapName);
            if (Directory.Exists(path))
            {
                list.AddRange(Directory.GetFiles(path, "*.sitemap").Select(u =>
                {
                    var fileName = Path.GetFileName(u);
                    return fileName.Remove(fileName.Length - ".sitemap".Length);
                }));
            }
            return list;
        }

        /// <summary>
        /// Returns relative path to a sitemap given.
        /// </summary>
        /// <param name="siteMapName">A sitemap name.</param>
        /// <returns>A relative path to the sitemap.</returns>
        public string GetSiteMapPath(string siteMapName)
        {
            string siteMapFile = null;
            if (siteMapName != DefaultSitemapName)
            {
                siteMapFile = "~/SiteMap/" + siteMapName + ".sitemap";
            }
            else
            {
                // Get sitemap provider name from config file
                var configSection = (SiteMapSection)(System.Configuration.ConfigurationManager.GetSection("system.web/siteMap"));

                if (!string.IsNullOrEmpty(configSection.DefaultProvider))
                {
                    foreach (ProviderSettings providerItem in configSection.Providers)
                    {
                        if (providerItem.Name == configSection.DefaultProvider)
                        {
                            siteMapFile = providerItem.Parameters["siteMapFile"];
                            break;
                        }
                    }
                }
            }

            // take default sitemap file name if not found
            if (string.IsNullOrEmpty(siteMapFile))
            {
                siteMapFile = "~/Web.sitemap";
            }

            // it should start from website root
            if (!siteMapFile.StartsWith("~/"))
            {
                if (!siteMapFile.StartsWith("/"))
                {
                    siteMapFile = "~/" + siteMapFile;
                }
                else
                {
                    siteMapFile = "~" + siteMapFile;
                }
            }
            return siteMapFile;
        }

        public TreeList GetSitmapTree(string siteMapName,bool exception=true)
        {
            string path = _settings.MapPath(GetSiteMapPath(siteMapName));

            TreeList tree = new TreeList();
            try
            {
                var s = new XmlSerializer(typeof(TreeList));
                using (TextReader r = new StreamReader(path))
                {
                    tree = (TreeList)s.Deserialize(r);
                    r.Close();
                }
            }
            catch
            {
                if (exception == true) { 
                    throw new Exception(String.Format("Cannot access sitemap file, check path in Web.config. Path : {0}", path));
                }
                else
                {
                    tree.Root = new TreeNode()
                    {
                        Key = "undef",
                        Url = "~/",
                        IsReference = false,
                        IsAnonymousOnly = false
                    };
                }
            }

            //tree._resourceFileName = Path.GetFileName(path);
            string resourceFileName = Path.GetFileName(path);

            //// add unique key for nodes without key
            //// and save updated sitemap
            //if (tree.Root.UpdateKeys(tree.Root))
            //    tree.Save(path, settings);

            var resProvider = new SiteMapResourceEditor(resourceFileName, _settings);
            tree.Root.LoadResources(_settings.AllowedLanguages, resProvider.Load());

            // fix roles
            tree.Root.FixRoles();

            return tree;

        }
    }
}