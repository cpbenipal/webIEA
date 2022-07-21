using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Web;

namespace Flexpage.Models
{
    public class FileUploaderModel
    {
        public string Name { get; set; }

        public string Controller { get; set; } = "Admin";
        public string Action { get; set; } = "UploadControl_ProcessingImageUpload";

        public string OnFileUploaded { get; set; }
        public string OnDropZoneEnter { get; set; }
        public string OnDropZoneLeave { get; set; }
        public string TextChanged { get; set; } = "fp_DevControlChanged";

        public UploadControlUploadMode UploadMode { get; set; } = UploadControlUploadMode.Standard;
        public bool ShowProgressPanel { get; set; } = true;
        public bool AutoStartUpload { get; set; } = true;

        public bool EnableDragAndDrop { get; set; } = false;
        public bool EnableFileList { get; set; } = false;
        public bool EnableMultiSelect { get; set; } = false;

        public bool ShowUploadButton { get; set; } = false;

        public string[] AllowedFileExtensions = new string[] {".jpg", ".jpeg", ".gif", ".png"};

        public string ExternalDropZoneID { get; set; } = "";
        public string DropZoneText { get; set; } = "";
        public string DialogTriggerID { get; set; } = null;

        public string TextBoxCssClass { get; set; } = "fileUpload";
        public string BrowseButtonCssClass { get; set; } = "fileUpload";

        public string DestinationFolder { get; set; } = "/Content/Images/User/";

        public string BrowseButtonText { get; set; }

        public bool ShowTextBox { get; set; } = true;

        public bool ShowAddRemoveButtons { get; set; } = false;
        public int Index { get; set; }
    }
}