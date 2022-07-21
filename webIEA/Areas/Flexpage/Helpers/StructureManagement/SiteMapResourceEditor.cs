using Flexpage.Code.StructureManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;

namespace Flexpage.Helpers.StructureManagement
{
    /// <summary>
    /// This class shoul replace SiteMapResource.
    /// The idea is to save all sitemap resources at ones together with sitemap file saving.
    /// </summary>
    public class SiteMapResourceEditor
    {
        private string _resourceFileName = null;
        private IEnumerable<string> _availableLanguages;
        private string _defaultLang;
        private bool _backupResourceFiles;

        public SiteMapResourceEditor(string sitemapFileName, Abstract.IFlexpageSettings setings)
        {
            if(string.IsNullOrEmpty(sitemapFileName))
            {
                sitemapFileName = "Web.sitemap";
            }
            _resourceFileName = setings.MapPath("~/SiteMap/" + sitemapFileName + "{0}.resx");
            _availableLanguages = setings.AllowedLanguages;
            _defaultLang = setings.DefaultLangCode;
            _backupResourceFiles = setings.BackupResourceFiles;

            if (_availableLanguages == null || _availableLanguages.Count() == 0)
            {
                throw new Exception("No available languages");
            }
            if (string.IsNullOrWhiteSpace(_defaultLang))
            {
                throw new Exception("Default language not found");
            }
        }

        public void Save(TreeList sitemapTree)
        {
            // save each language separately
            foreach (var lang in _availableLanguages)
            {
                using (var ms = new MemoryStream())
                using (var wr = new ResXResourceWriter(ms))
                {
                    // don't need to generate the root resources
                    writeNodeResources(sitemapTree.Root, lang, wr);
                    wr.Generate();

                    string resourceFilePath = getResourceFileName(lang);

                    // make a backup of the resources file if need
                    if (File.Exists(resourceFilePath) && _backupResourceFiles)
                    {
                        string backupFilePath = Path.Combine(Path.GetDirectoryName(resourceFilePath), "Backup");
                        if(!Directory.Exists(backupFilePath))
                        {
                            Directory.CreateDirectory(backupFilePath);
                        }

                        string backupName = Path.GetFileNameWithoutExtension(resourceFilePath);
                        backupName += "." + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak";

                        backupFilePath = Path.Combine(backupFilePath, backupName);
                        File.Copy(resourceFilePath, backupFilePath, true);
                    }

                    // save resources into file
                    using (var fs = new FileStream(resourceFilePath, FileMode.Create))
                    {
                        ms.WriteTo(fs);
                    }
                }
            }
        }

        public Dictionary<string, Dictionary<string, string>> Load()
        {
            var dictLangs = new Dictionary<string, Dictionary<string, string>>();

            foreach (var lang in _availableLanguages)
            {
                var path = getResourceFileName(lang);
                if (File.Exists(path))
                    using (var rd = new ResXResourceReader(path))
                    {
                        var dictLang = new Dictionary<string, string>();
                        rd.UseResXDataNodes = false;
                        foreach (DictionaryEntry node in rd)
                            if (!dictLang.ContainsKey(node.Key.ToString().ToLower()))
                                dictLang.Add(node.Key.ToString().ToLower(), node.Value.ToString());
                        dictLangs.Add(lang, dictLang);
                    }
            }
            return dictLangs;
        }

        private void writeNodeResources(TreeNode parentNode, string lang, ResXResourceWriter wr)
        {
            foreach(TreeNode node in parentNode.Nodes)
            {
                addResource(node.AllTitles, lang, wr, node.Key, "Title");
                addResource(node.AllDescriptions, lang, wr, node.Key, "Description");
                writeNodeResources(node, lang, wr);
            }
        }

        private void addResource(Dictionary<string, string> dict, string lang, ResXResourceWriter wr, string resourceKey, string resourcePostfix)
        {
            string val;
            if(dict.TryGetValue(lang, out val))
            {
                wr.AddResource(resourceKey + "." + resourcePostfix, val);
            }
        }

        private string getResourceFileName(string lang)
        {
            return string.Format(_resourceFileName, string.IsNullOrEmpty(lang) || lang == _defaultLang ? "" : "." + lang);
        }
    }
}