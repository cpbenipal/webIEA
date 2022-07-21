using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Flexpage.Models;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using FlexPage2.Areas.Flexpage.Infrastructure;
using DevExpress.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Flexpage.Controllers
{
    
    public class CSSSettingsController : Controller
    {
        private readonly ICSSSettingsProvider _cssSettingsProvider;
        public CSSSettingsController(ICSSSettingsProvider cssSettingsProvider)
        {
            _cssSettingsProvider = cssSettingsProvider;
        }
        [FlexpageAdmin]
        public ActionResult CSSSettings(string alias)
        {
            var model= _cssSettingsProvider.Load();

            return GetEditorFor(model);
        }
        [FlexpageAdmin]
        protected PartialViewResult GetEditorFor(CSSSettingsModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Admin/CSSSettings/CSSSettings.cshtml", model);
        }
        [FlexpageAdmin]
        public PartialViewResult UpdateCSSSettings(CSSSettingsModel model, string command, string parameters)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string c = command.ToLower().Trim();
                    if (c == "save")
                    {
                        _cssSettingsProvider.Save(model);
                    }
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return GetEditorFor(model);
        }
        [FlexpageAdmin]
        [HttpPost]
        public PartialViewResult LoadCssSettingFile(int index,CssSettingFileModel model)
        {
            ViewBag.Index = index;
            return PartialView("~/Areas/Flexpage/Views/Admin/CSSSettings/CSSSettingsFile.cshtml", model);
        }
        [FlexpageAdmin]
        [ValidateInput(false)]
        public ActionResult UploadCSSSettingsFile(string folderPath, string uploadControlName,int Index)
        {
            UploadControlExtension.GetUploadedFiles(uploadControlName, null, (s, e) =>
            {
                if (e.UploadedFile.IsValid)
                {
                   var model=new CssSettingFileModel()
                    {
                        Name = e.UploadedFile.FileName,
                        Path = folderPath + e.UploadedFile.FileName,
                        Index= Index,
                        IsEnabled = true,
                        IsSystem = false
                    };
                    var json = new JavaScriptSerializer().Serialize(model);
                    e.CallbackData = json;
                    string resultFileName = folderPath + e.UploadedFile.FileName.Replace("&", string.Empty);
                    string resultFilePath = HttpContext.Server.MapPath(resultFileName);
                    int idx = 0;
                    while (System.IO.File.Exists(resultFilePath))
                    {
                        resultFileName = string.Format("{2}{0}_{1}", ++idx, e.UploadedFile.FileName, folderPath);
                        resultFilePath = HttpContext.Server.MapPath(resultFileName);
                    }
                    var f = HttpContext.Server.MapPath(folderPath);
                    DirectoryInfo dirInfo = new DirectoryInfo(f);
                    if (!dirInfo.Exists)
                    {
                        dirInfo.Create();
                    }
                    e.UploadedFile.SaveAs(resultFilePath, true);
                }
            });
            return null;
        }
        [FlexpageAdmin]
        [ValidateInput(false)]
        public ActionResult UploadCustomCss(string folderPath, string uploadControlName)
        {
            UploadControlExtension.GetUploadedFiles(uploadControlName, null, (s, e) =>
            {
                if (e.UploadedFile.IsValid)
                {
                    using (StreamReader reader = new StreamReader(e.UploadedFile.FileContent))
                    {
                        e.CallbackData = reader.ReadToEnd();
                    }
                }
            });
            return null;
        }
        public MvcHtmlString GenerateCustomCss()
        {
            string headers = GetPageHeaders();
            return new MvcHtmlString(headers);
        }
        private string GetPageHeaders()
        {
            
            CSSSettingsModel cssSettingsModel = _cssSettingsProvider.Load();
            var result = "";
            if (!cssSettingsModel.DisableCustomCss)
            {
                cssSettingsModel.CssSettingFiles.ForEach(file=>
                {
                    result += string.Format("<link href=\"{0}\" rel=\"stylesheet\">", file.Path);
                });
                result += string.Format("<style type=\"text/css\">{0}</style>", cssSettingsModel.CustomCSS);
            }
            return result;
        }
    }
}