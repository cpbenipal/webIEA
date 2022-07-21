using DevExpress.Utils.Extensions;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using FlexPage2.Areas.Flexpage.Helpers;
using Newtonsoft.Json;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.Permissions.Abstract;
using Pluritech.Properties.Abstract;
using Pluritech.Properties.Abstract.DTO;
using Pluritech.Shared.Abstract;
using Pluritech.Shared.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class ExportController : Controller
    {
        private readonly IFlexpageRepository _repository;
        private readonly IContactProvider _contactProvider;
        protected readonly IPermissionsService _permissionsService;
        protected readonly IPropertyProvider _propertyService;
        private readonly IUserSettingsProvider _userSettings;
        private readonly IObjectQueryBuilder _contactsQueryBuilder;

        public ExportController(IFlexpageRepository repository, [Ninject.Named("ContactsQueryBuilder")] IObjectQueryBuilder contactsQueryBuilder,
            IContactProvider contactProvider, IPermissionsService permissionsService, IPropertyProvider propertyService,
            IUserSettingsProvider userSettings)
        {
            _repository = repository;
            _contactsQueryBuilder = contactsQueryBuilder;
            _contactProvider = contactProvider;
            _permissionsService = permissionsService;
            _propertyService = propertyService;
            _userSettings = userSettings;
        }

        public ActionResult ShowExportSettingsDialog(int folderId, string exportSearchQuery = null, string xml = null)
        {
            ViewData["folderId"] = folderId;
            var folder = _repository.GetFolders().FirstOrDefault(w => w.ID == folderId);

            var data = new ContactsTreeList();
            data.ExportSearchQuery = exportSearchQuery;
            data.FolderID = folderId;
            contactsTreeListType type; //0 - both types, 1 - persons, 2 - companies

            if (String.IsNullOrEmpty(xml))
            {
                var contacts = FindContacts(folder, exportSearchQuery);
                if (contacts.Any(c => c.Type == "CompanyInfo"))
                {
                    if (contacts.Any(c => c.Type == "PersonInfo"))
                    {
                        type = 0;
                    }
                    else type = (contactsTreeListType)2;
                }
                else type = (contactsTreeListType)1;
            }
            else
            {
                HttpCookie cookie = Request.Cookies[xml];
                var query = cookie.Value;
                query = HttpUtility.UrlDecode(query);
                var result = _contactProvider.GetTypeXML(query);
                type = (contactsTreeListType)result;
            }

            data.type = type;

            switch (type)
            {
                case contactsTreeListType.All:
                    data.SelectColumnTreeList = _propertyService.CachedObjectPropertiesAvailable(folderId);
                    break;
                case contactsTreeListType.Person:
                    data.SelectColumnTreeList = _propertyService.CachedObjectPropertiesAvailable(folderId).Where(p => p.ParentType == ParentTypes.Person || p.Id <= 2).ToList();
                    break;
                case contactsTreeListType.Company:
                    data.SelectColumnTreeList = _propertyService.CachedObjectPropertiesAvailable(folderId).Where(p => p.ParentType == ParentTypes.Company || p.Id == 3 || p.Id == 4).ToList();
                    break;
            }
            var user = User.Identity.Name;
            var savedProperty = _userSettings.LoadUserProperty(user, eUserPropertyType.ContactExportSelection);
            try
            {
                if (savedProperty.PropertyValueInfo.Value != null)
                {
                    int[] selectedItems = JsonConvert.DeserializeObject<int[]>(savedProperty.PropertyValueInfo.Value);
                    data.SelectColumnTreeList.ForEach(n => { n.IsSelect = selectedItems.Contains(n.Id); });
                }
            }
            catch
            {
                // ignored
            }
            return View("~/Areas/Flexpage/Views/Shared/EditorTemplates/FolderContent/SelectColumns.cshtml", data);
        }
        public ActionResult SelectColumns(int folderId, contactsTreeListType type, string exportSearchQuery = null)
        {
            var folder = _repository.GetFolders().FirstOrDefault(w => w.ID == folderId);

            var data = new ContactsTreeList();
            data.ExportSearchQuery = exportSearchQuery;
            data.FolderID = folderId;
            data.type = type;

            switch (type)
            {
                case contactsTreeListType.All:
                    data.SelectColumnTreeList = _propertyService.CachedObjectPropertiesAvailable(folderId);
                    break;
                case contactsTreeListType.Person:
                    data.SelectColumnTreeList = _propertyService.CachedObjectPropertiesAvailable(folderId).Where(p => p.ParentType == ParentTypes.Person || p.Id <= 2).ToList();
                    break;
                case contactsTreeListType.Company:
                    data.SelectColumnTreeList = _propertyService.CachedObjectPropertiesAvailable(folderId).Where(p => p.ParentType == ParentTypes.Company || p.Id == 3 || p.Id == 4).ToList();
                    break;
            }
            var user = User.Identity.Name;
            var savedProperty = _userSettings.LoadUserProperty(user, eUserPropertyType.ContactExportSelection);
            try
            {
                if (savedProperty.PropertyValueInfo.Value != null)
                {
                    int[] selectedItems = JsonConvert.DeserializeObject<int[]>(savedProperty.PropertyValueInfo.Value);
                    data.SelectColumnTreeList.ForEach(n => { n.IsSelect = selectedItems.Contains(n.Id); });
                }
            }
            catch
            {
                // ignored
            }
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FolderContent/_SelectColumnTreeList.cshtml", data);
        }

        private IObjectQueryBuilder FindContacts(Folder folder = null, string exportSearchQuery = null, string Xml = null)
        {
            var filterBy = new List<FilterDesciption>();

            filterBy.Add(new FilterDesciption() { Table = "ShowContacts", Value = (int)eContactType.None });
            filterBy.Add(new FilterDesciption()
            {
                Table = "Folder",
                Field = "Name",
                Value = folder?.Name ?? "\\",
                FilterMode = eFilterMode.Equal
            });

            if (!string.IsNullOrEmpty(Xml))
            {
                Xml = _contactProvider.BPMNXMLToXML(Xml);
                var res = _contactProvider.SearchContacts(Xml);
                var contacts = res.ToList();
                var type = _contactProvider.GetTypeXML(Xml);
                filterBy.Add(new FilterDesciption()
                {
                    Field = type.ToString(),
                    Table = "SearchResult",
                    Value = contacts
                });
            }

            if (!string.IsNullOrEmpty(exportSearchQuery))
            {
                filterBy.Add(new FilterDesciption()
                {
                    Table = "Search",
                    Field = "Search",
                    Value = exportSearchQuery,
                    FilterMode = eFilterMode.Contains
                });
            }

            return (_contactsQueryBuilder.NewQuery().FilterBy(filterBy));
        }

        [ValidateInput(false)]
        public ActionResult ExportContacts(ContactsTreeList obj = null, string exportSearchQuery = null)
        {
            if (obj?.SelectColumnTreeList.Count != 0)
            {
                Session["exportContacts"] = obj;
                Session["exportSearchQuery"] = exportSearchQuery;
                return Content(string.Empty);
            }

            if (Session["exportContacts"] == null)
                throw new Exception("Wrong Parameters.");

            obj = (ContactsTreeList)Session["exportContacts"];
            exportSearchQuery = (string)Session["exportSearchQuery"];

            Session["exportContacts"] = null;
            Session["exportSearchQuery"] = null;

            if (obj.SelectColumnTreeList.Count == 0)
                return Content("No columns were selected.");

            string contentType = MimeMapping.GetMimeMapping("Contacts");
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "Contacts.xlsx",
                Inline = false
            };

            var folder = _repository.GetFolders().FirstOrDefault(w => w.ID == obj.FolderID);

            var serializedSelectedNodes = JsonConvert.SerializeObject(obj.SelectColumnTreeList
                .Where(n => n.IsSelect)
                .Select(n => n.Id)
                .ToArray());
            var selectedNodes = new PropertyValueInfoModel()
            {
                Value = serializedSelectedNodes,
                PropertyType = eUserPropertyType.ContactExportSelection
            };
            _userSettings.SaveUserProperty(new UserPropertyModel()
            {
                PropertyValueInfo = selectedNodes,
                UserName = User.Identity.Name
            });

            //**************************************************
            var query = FindContacts(folder, exportSearchQuery, obj.Xml);
            var idsPerson = new List<int>();
            var idsCompany = new List<int>();

            query.ObjectsList().ForEach(item =>
            {
                item.Properties.ForEach(prop =>
                {
                    if (prop.Name.Equals("PersonCompany.ID"))
                        if (item.Type == "CompanyInfo")
                            idsCompany.Add((int)prop.Value);
                        else
                            idsPerson.Add((int)prop.Value);
                });
            });

            //******************************************************
            Response.AppendHeader("Content-Disposition", cd.ToString());

            var dataExportPerson = new List<ObjectEntity>();
            var dataExportCompany = new List<ObjectEntity>();
            var objectEntitiesPerson = _repository.GetPersons()?.ToList().Where(w => idsPerson.Contains(w.PersonID)).Select(s => new Domain.Entities.ObjectEntity()
            { PersonShortcut = s.Person.PersonObject })?.ToList();
            var objectEntitiesCompany = _repository.GetCompanies()?.ToList().Where(c => idsCompany.Contains(c.CompanyID)).Select(s => new ObjectEntity()
            { CompanyShortcut = s.Company.CompanyObject })?.ToList();

            if (objectEntitiesPerson != null)
            {
                foreach (var entity in objectEntitiesPerson)
                {
                    if (entity.PersonShortcut != null)
                    {
                        if (dataExportPerson.FirstOrDefault(w => w.PersonShortcut.PersonID == entity.PersonShortcut.PersonID) ==
                            null)
                        {
                            dataExportPerson.Add(entity);
                        }
                    }

                }
            }

            if (objectEntitiesCompany != null)
            {
                foreach (var entity in objectEntitiesCompany)
                {
                    if (entity.CompanyShortcut != null)
                    {
                        {
                            if (dataExportCompany.FirstOrDefault(w => w.CompanyShortcut.CompanyID == entity.CompanyShortcut.CompanyID) ==
                                null)
                            {
                                dataExportCompany.Add(entity);
                            }
                        }
                    }
                }
            }

            var ecx = new ExportContactsHelper(_repository, _contactProvider).Export(obj.SelectColumnTreeList, dataExportPerson, dataExportCompany);

            return File(ecx, contentType);
        }
    }
}