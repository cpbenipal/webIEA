using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class FileUploadModel
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public string CurrentCultureMessage { get; set; }
        public List<FileTuple> FilesData { get; }

        public FileUploadModel()
        {
            this.FilesData = new List<FileTuple>();
        }
    }
    public class FileTuple
    {
        //public int FileID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}