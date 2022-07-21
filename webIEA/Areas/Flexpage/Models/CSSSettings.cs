using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class FileCSS
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int Order { get; set; }
    }
    public class CSSSettings
    {
        public bool Enable { get; set; }
        public List<FileCSS> FilesCSS { get; set; }
        public string CustomCSS { get; set; }
        public CSSSettings()
        {
            FilesCSS = new List<FileCSS>() {
                new FileCSS() { ID=1, Name="bootstrap-3.css", Order=10 },
                new FileCSS() { ID = 2, Name = "bootstrap-4.css", Order = 1 }
            };
        }
    }
}