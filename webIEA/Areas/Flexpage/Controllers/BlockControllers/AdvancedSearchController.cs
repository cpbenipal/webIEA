using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Models;
using Pluritech.Contact.Abstract;
using Pluritech.FileSystem.Abstract;
using Pluritech.FileSystem.Abstract.DTO;
using Pluritech.Properties.Abstract;
using Pluritech.Properties.Abstract.DTO;
using Pluritech.Shared.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;

namespace Flexpage.Controllers
{
    public class AdvancedSearchController : Controller
    {
        private readonly IFlexpageRepository _repository;
        private readonly IFlexpageSettings _settings;
        private readonly IFlexpage _flexpageProcessor;
        private readonly IContactProvider _contactProvider;
        private readonly IFileContentProcessor _fileService;
        protected readonly IPropertyProvider _propertyService;

        public AdvancedSearchController(IFlexpageRepository repository, IFlexpageSettings settings, IFlexpage flexpageProcessor,
            IContactProvider contactProvider, IFileContentProcessor fileService, IPropertyProvider propertyService)
        {
            _flexpageProcessor = flexpageProcessor;
            _repository = repository;
            _settings = settings;
            _contactProvider = contactProvider;
            _fileService = fileService;
            _propertyService = propertyService;
        }

        public ActionResult ContactsAdvancedSearch(string alias, string folder = "/Areas/Flexpage/Content/diagram")
        {
            try
            {
                var fileName = User.Identity.Name;
                var extension = ".xml";
                var folderSave = WebConfigurationManager.AppSettings["FP:AdvancedSearchDiagramFiles"];
                var start = "";
                string resultFileName = folderSave + fileName + extension;
                string resultFilePath = HttpContext.Server.MapPath(resultFileName);
                try
                {
                    using (StreamReader sr = new StreamReader(resultFilePath))
                    {
                        start = sr.ReadToEnd();
                    }
                    // start = _contactProvider.XMLToBPMNXML(start);
                }
                catch (Exception)
                {

                }
                ContactsAdvancedSearchModel model = new ContactsAdvancedSearchModel(_settings, _flexpageProcessor);
                model.Load(_repository, alias,
                    _contactProvider.GetContactFields(ObjectTypeEnum.PersonObject),
                    _contactProvider.GetContactFields(ObjectTypeEnum.CompanyObject),
                    _contactProvider.GetContactFields(ObjectTypeEnum.Folder),
                    _contactProvider.SearchLinkTypes(ObjectTypeEnum.PersonObject, ObjectTypeEnum.PersonObject),
                    _contactProvider.SearchLinkTypes(ObjectTypeEnum.PersonObject, ObjectTypeEnum.CompanyObject),
                    _contactProvider.SearchLinkTypes(ObjectTypeEnum.PersonObject, ObjectTypeEnum.Folder),
                    _contactProvider.SearchLinkTypes(ObjectTypeEnum.CompanyObject, ObjectTypeEnum.PersonObject),
                    _contactProvider.SearchLinkTypes(ObjectTypeEnum.CompanyObject, ObjectTypeEnum.CompanyObject),
                    _contactProvider.SearchLinkTypes(ObjectTypeEnum.CompanyObject, ObjectTypeEnum.Folder), folder,
                    start, true);
                return PartialView("~/Areas/Flexpage/Views/Flexpage/ContactsAdvancedSearch.cshtml", model);
            }
            catch (Exception ex)
            {
#if DEBUG
                return PartialView("~/Areas/Flexpage/Views/Shared/Error.cshtml", String.Format("Sorry, there is something wrong with block '{0}'. Please contact your administrator. <br><b>Message:</b> {1} <br><b>Stacktrace:</b> {2}", alias, ex.Message, ex.StackTrace));
#else
                return PartialView("~/Areas/Flexpage/Views/Shared/Error.cshtml", String.Format("Sorry, there is something wrong with block '{0}'. Please contact your administrator.", alias));
#endif
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ContactsAdvancedSearch_Result()
        {
            ViewBag.IsAdmin = IsAdmin();
            var query = "";
            var queryContacts = _repository.QueryContacts;
            List<FilterDesciption> filterDesciptions = new List<FilterDesciption>();
            List<OrderByDesciption> orderByDesciptions = new List<OrderByDesciption>();
            List<ObjectInfo> search = new List<ObjectInfo>();
            string xmlString = "";
            try
            {
                xmlString = Request.Params.Get("xml");
                query = _contactProvider.BPMNXMLToXML(xmlString);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = "query:" + query + " " + e.Message;
            }

            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactsAdvancedSearch/_ContactsAdvancedSearchResult.cshtml", query);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult ContactsAdvancedSearch_SaveXmlChanges()
        {
            ViewBag.IsAdmin = IsAdmin();
            var fileName = User.Identity.Name;
            var extension = ".xml";
            var folder = WebConfigurationManager.AppSettings["FP:AdvancedSearchDiagramFiles"];
            try
            {
                string resultFileName = folder + fileName + extension;
                string resultFilePath = HttpContext.Server.MapPath(resultFileName);

                var f = HttpContext.Server.MapPath(folder);
                DirectoryInfo dirInfo = new DirectoryInfo(f);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                if (!string.IsNullOrWhiteSpace(Request.Params.Get("xml")))
                {
                    string xmlString = Request.Params.Get("xml").Replace("\\\"", "\"");
                    //query = _contactProvider.BPMNXMLToXML(xmlString);
                    if (System.IO.File.Exists(resultFilePath))
                    {
                        System.IO.File.Delete(resultFilePath);
                    }
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(resultFilePath))
                    {
                        file.Write(xmlString);
                    }
                }
                else if (System.IO.File.Exists(resultFilePath))
                {
                    System.IO.File.Delete(resultFilePath);
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = new { title = "Error import", message = e.Message } });
            }

            return Json(new { success = true });
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult ContactsAdvancedSearch_Export()
        {
            var name = "contact-advanced-search";
            var extension = ".xml";
            var query = "";
            string path = "";
            try
            {
                path = Request.Params.Get("path");
                string xmlString = Request.Params.Get("xml");
                query = _contactProvider.BPMNXMLToXML(xmlString);
                if (!string.IsNullOrEmpty(path))
                {
                    if (!int.TryParse(Request.Params.Get("folderId"), out var blockID))
                    {
                        throw new ArgumentNullException();
                    }
                    bool overwrite = false;
                    if (!string.IsNullOrEmpty(Request.Params.Get("overwrite")))
                    {
                        overwrite = Request.Params.Get("overwrite").Trim().ToLower() == "true";
                    }
                    if (!string.IsNullOrEmpty(Request.Params.Get("name")))
                    {
                        name = Request.Params.Get("name").Trim();
                    }
                    else
                    {
                        return Json("{\"success\": false,\"error\": { \"title\" : \"SAVE DIAGRAM ERROR\",\"message\" : \"The name can't be blank\"}}");
                    }
                    var filesExist = new List<string>();
                    var stream = new MemoryStream();
                    var writer = new StreamWriter(stream);
                    writer.Write(query);
                    writer.Flush();
                    stream.Position = 0;

                    var namedStream = new NamedStream()
                    {
                        Name = name + extension,
                        Stream = stream,
                        Overwrite = overwrite,
                        DateModification = DateTime.UtcNow
                    };

                    if (_fileService.UploadFile(path, namedStream, "") == false)
                    {
                        return Json("{\"success\": false,\"error\": { \"title\" : \"SAVE DIAGRAM ERROR\",\"message\" : \"" + name + extension + " already exists. Do you want to replace it?\"}}");
                    }
                }
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"SAVE DIAGRAM ERROR\",\"message\" : \"" + e.Message.Replace("\"", "\\\"") + "\"}}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var file = _repository.QueryFiles.FilterBy(new List<FilterDesciption>() {
                        new FilterDesciption()
                        {
                            Table="FileShortcut", Field="Name", Value=name
                        },
                        new FilterDesciption()
                        {
                            Table="File", Field="Extension", Value=extension
                        },
                        new FilterDesciption()
                        {
                            Table="Folder", Field="Name", Value=path
                        }
                    })?.FirstOrDefault();
                    if (file != null)
                    {
                        if (_propertyService.GetObjectCustomProperties(file.ID)?.FirstOrDefault(f => f.Name == "ContactsQuery") == null)
                        {
                            var CQ = _propertyService.EnumerateAllProperties()?.FirstOrDefault(p => p.Name == "ContactsQuery");
                            if (CQ != null)
                            {
                                _propertyService.SaveObjectCustomPropertiesValues(new CustomPropertyModel()
                                {
                                    ObjectID = file.ID,
                                    Properties = new List<CustomPropertyInfo>() {
                        new CustomPropertyInfo() { ID= CQ.ID, Name=CQ.Name} }
                                });
                            }
                        }
                    }
                }
            }

            return Json("{\"success\": true,\"result\" : \"" + query.Replace("\"", "\\\"") + "\"}");
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult ContactsAdvancedSearch_Import()
        {
            var query = "";
            try
            {
                string xmlString = Request.Params.Get("xml").Replace("\\\"", "\"");
                string bpmnXml = Request.Params.Get("bpmnXml");
                query = _contactProvider.XMLToBPMNXML(xmlString, bpmnXml);
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Error import\",\"message\" : \"" + query.Replace("\"", "\\\"") + " " + e.Message + "\"}}");
            }

            return Json("{\"success\": true,\"result\" : \"" + query.Replace("\"", "\\\"") + "\"}");
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult ContactsAdvancedSearch_Open(int? fileID)
        {
            var query = "";
            try
            {
                var file = _fileService.DownloadFile((int)fileID, _fileService.GetFileVersionHistories((int)fileID).OrderByDescending(s => s.RevisionID)
                .Select(s => s.RevisionID)
                .FirstOrDefault());
                XmlDocument xmlDocument = new XmlDocument();


                string bpmnXml = Request.Params.Get("bpmnXml");
                query = _contactProvider.XMLToBPMNXML(file.Stream, bpmnXml);
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Error open xml file\",\"message\" : \"" + query.Replace("\"", "\\\"") + " " + e.Message + "\"}}");
            }

            return Json("{\"success\": true,\"result\" : \"" + query.Replace("\"", "\\\"") + "\"}");
        }

        protected bool IsAdmin()
        {
            return _settings.IsCmsAdmin();
        }

        //protected bool IsContactsAdmin()
        //{
        //    return _permissionsService.IsContactsAdmin(User.Identity.Name);
        //}

        //protected bool IsPWAdmin()
        //{
        //    return _userGeneralPermissionProvider.IsPWAdmin();
        //}
    }
}