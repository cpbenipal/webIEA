using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Code.Helpers;
using Flexpage.Domain.Abstract;
using Flexpage.Models;
using Newtonsoft.Json;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.Permissions.Abstract;
using Pluritech.Shared.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class ContactsEnumerationController : Controller
    {
        private readonly IFlexpageRepository _repository;
        private readonly IFlexpageSettings _settings;
        private readonly IFlexpage _flexpageProcessor;
        private readonly IContactProvider _contactProvider;
        protected readonly IPermissionsService _permissionsService;

        public ContactsEnumerationController(IFlexpageRepository repository, IFlexpageSettings settings, IFlexpage flexpageProcessor, 
            IContactProvider contactProvider, IPermissionsService permissionsService,
            [Ninject.Named("FoldersQueryBuilder")] IObjectQueryBuilder _foldersQueryBuilder,
            [Ninject.Named("ContactsQueryBuilder")] IObjectQueryBuilder _contactsQueryBuilder)
        {
            _flexpageProcessor = flexpageProcessor;
            _repository = repository;
            _repository.QueryContacts = _contactsQueryBuilder;
            _repository.QueryFolders = _foldersQueryBuilder;
            _settings = settings;
            _contactProvider = contactProvider;
            _permissionsService = permissionsService;
        }

        public ActionResult ContactsEnumerationGrid(int ID, string selectFolderName = null, TypeContextMenu typeContextMenu = TypeContextMenu.Block,
            int? ContactID = null, int? ContactShortcutID = null, string xml = null, eContactType ShowContacts = eContactType.None, bool ShowSelectedContactsGrid = false, string SelectedValues = "")
        {
            ContactsEnumerationModel model = ContactsEnumerationModel(ID, null, true, selectFolderName, typeContextMenu, xml, ContactID, ContactShortcutID, ShowContacts, ShowSelectedContactsGrid, SelectedValues);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactsEnumeration/_ContactsEnumerationGrid.cshtml", model);
        }

        public ActionResult ContactsEnumeration_SortingAction(DevExpress.Web.Mvc.GridViewColumnState column, int ID, string selectFolderName, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string xml = null, bool ShowSelectedContactsGrid = false, string SelectedValues = "")
        {
            var viewModel = GridViewExtension.GetViewModel("fp_ContactsEnumeration_Grid" + ID.ToString());
            viewModel.SortBy(column, true);
            var SearchPanelFilter = viewModel.SearchPanel.Filter;
            viewModel.SearchPanel.Filter = "";
            return ContactsEnumeration_GridCoreAction(ID, viewModel, selectFolderName, SearchPanelFilter, typeContextMenu, xml, ShowSelectedContactsGrid, SelectedValues);
        }

        public ActionResult ContactsEnumeration_PagingAction(GridViewPagerState pager, int ID, string selectFolderName, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string xml = null, bool ShowSelectedContactsGrid = false, string SelectedValues = "")
        {
            var viewModel = GridViewExtension.GetViewModel("fp_ContactsEnumeration_Grid" + ID.ToString());
            var SearchPanelFilter = viewModel.SearchPanel.Filter;
            viewModel.SearchPanel.Filter = "";
            viewModel.ApplyPagingState(pager);

            return ContactsEnumeration_GridCoreAction(ID, viewModel, selectFolderName, SearchPanelFilter, typeContextMenu, xml, ShowSelectedContactsGrid, SelectedValues);
        }

        public ActionResult ContactsEnumeration_FilteringAction(GridViewFilteringState filteringState, int ID, string selectFolderName, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string xml = null, bool ShowSelectedContactsGrid = false, string SelectedValues = "")
        {
            var viewModel = GridViewExtension.GetViewModel("fp_ContactsEnumeration_Grid" + ID.ToString());
            var SearchPanelFilter = filteringState.SearchPanelFilter;
            filteringState.SearchPanelFilter = "";
            viewModel.ApplyFilteringState(filteringState);
            filteringState.SearchPanelFilter = SearchPanelFilter;
            return ContactsEnumeration_GridCoreAction(ID, viewModel, selectFolderName, SearchPanelFilter, typeContextMenu, xml, ShowSelectedContactsGrid, SelectedValues);
        }

        public ActionResult ContactsEnumeration_Update(int ID, string selectFolderName = null, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string xml = null, bool initPerformCallback = true)
        {
            ViewBag.InitPerformCallback = initPerformCallback;
            return ContactsEnumeration(ID, null, false, selectFolderName, typeContextMenu, xml);
        }

        public ActionResult ContactsEnumeration(int ID, ContactsEnumerationModel model = null, bool ProcessCustomBinding = true, string selectFolderName = null, TypeContextMenu typeContextMenu = TypeContextMenu.Block,
            string xml = null, int? contactID = null, int? contactShortcutID = null, eContactType ShowContacts = eContactType.None)
        {
            model = ContactsEnumerationModel(ID, model, false, selectFolderName, typeContextMenu, xml, contactID, contactShortcutID, ShowContacts);

            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactsEnumeration/_ContactsEnumeration.cshtml", model);
        }

        [HttpPost]
        public JsonResult UpdateContactsEnumerationFolder(string parameters /*, int contactID, eContactType contactType,string path,string command*/)
        {
            List<string> names = new List<string>();
            try
            {

                ModelState.Clear();

                List<ContactsEnumerationJson> parameterObjects = JsonConvert.DeserializeObject<List<ContactsEnumerationJson>>(parameters);

                parameterObjects.ForEach(item =>
                {

                    string c = item.Command.ToLower().Trim();
                    var folder = _repository.GetFolder(item.Path);

                    switch (c)
                    {
                        case "add":
                            {
                                var person = _repository.GetPersonShortcut(item.ContactID)?.Person;
                                var company = _repository.GetCompanyShortcut(item.ContactID)?.Company;
                                if (person != null)
                                {
                                    try
                                    {
                                        _contactProvider.AddPersonToFolder(person.ID, folder == null ? 0 : folder.ID);
                                    }
                                    catch (Exception e)
                                    {
                                        names.Add(string.IsNullOrEmpty(person.Name1) ? person.Name2 : (person.Name1 + (string.IsNullOrEmpty(person.Name2) ? "" : ", " + person.Name2)) + " - " + e.Message.Split('.')?.FirstOrDefault());
                                    }
                                }
                                else if (company != null)
                                {
                                    try
                                    {
                                        _contactProvider.AddCompanyToFolder(company.ID, folder == null ? 0 : folder.ID);
                                    }
                                    catch (Exception e)
                                    {

                                        names.Add(string.IsNullOrEmpty(company.Name1) ? company.Name2 : (company.Name1 + (string.IsNullOrEmpty(company.Name2) ? "" : ", " + company.Name2)) + " - " + e.Message.Split('.')?.FirstOrDefault());
                                    }
                                }
                            }
                            break;
                        case "remove":
                            if (item.ContactType == eContactType.Person)
                            {
                                try
                                {
                                    _contactProvider.DeletePersonFromFolderByID(item.ContactID, folder == null ? 0 : folder.ID);
                                }
                                catch (Exception e)
                                {
                                    var person = _repository.GetPerson(item.ContactID);
                                    names.Add(string.IsNullOrEmpty(person.Name1) ? person.Name2 : (person.Name1 + (string.IsNullOrEmpty(person.Name2) ? "" : ", " + person.Name2)) + " - " + e.Message.Split('.')?.FirstOrDefault());
                                }
                            }
                            else if (item.ContactType == eContactType.Company)
                            {
                                try
                                {
                                    _contactProvider.DeleteCompanyFromFolderByID(item.ContactID, folder == null ? 0 : folder.ID);
                                }
                                catch (Exception e)
                                {
                                    var company = _repository.GetCompany(item.ContactID);
                                    names.Add(string.IsNullOrEmpty(company.Name1) ? company.Name2 : (company.Name1 + (string.IsNullOrEmpty(company.Name2) ? "" : ", " + company.Name2)) + " - " + e.Message.Split('.')?.FirstOrDefault());
                                }
                            }
                            MemoryCache memoryCache = MemoryCache.Default;
                            memoryCache.Dispose();
                            break;
                        case "delete":
                            if (IsContactsAdmin())
                            {
                                if (item.ContactType == eContactType.Person)
                                {
                                    try
                                    {
                                        _contactProvider.DeleteContact(null, item.ContactID);
                                    }
                                    catch (Exception e)
                                    {
                                        var person = _repository.GetPerson(item.ContactID);
                                        names.Add(string.IsNullOrEmpty(person.Name1) ? person.Name2 : (person.Name1 + (string.IsNullOrEmpty(person.Name2) ? "" : ", " + person.Name2)) + " - " + e.Message.Split('.')?.FirstOrDefault());
                                    }
                                }
                                else if (item.ContactType == eContactType.Company)
                                {
                                    try
                                    {
                                        _contactProvider.DeleteContact(item.ContactID, null);
                                    }
                                    catch (Exception e)
                                    {
                                        var company = _repository.GetCompany(item.ContactID);
                                        names.Add(string.IsNullOrEmpty(company.Name1) ? company.Name2 : (company.Name1 + (string.IsNullOrEmpty(company.Name2) ? "" : ", " + company.Name2)) + " - " + e.Message.Split('.')?.FirstOrDefault());
                                    }
                                }

                                _repository.ApplyChanges();
                            }
                            else
                            {
                                throw new Exception("Not enough permissions");
                            }
                            break;
                    }
                });
                if (names.Count > 0)
                {
                    throw new Exception("Contact(s):<br />" + string.Join(";<br />", names.ToArray()) + ".");
                }
                return Json("{\"success\": true}");
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Error in changing contacts attached to a folder\",\"message\" : \"" + e.Message + "\"}}");
            }

        }

        [HttpPost]
        public PartialViewResult CheckAddPerson(PersonViewModel contact)
        {
            if (ModelState.IsValid)
            {
            }
            else
            {
                throw new Exception("Please fill in all fields");
            }
            var model = new ContactAddModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { }, "", true);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/ContactAdd.cshtml", model);
        }

        [HttpPost]
        public PartialViewResult CheckAddCompany(CompanyViewModel contact)
        {
            if (ModelState.IsValid)
            {
            }
            else
            {
                throw new Exception("Please fill in all fields");
            }
            var model = new ContactAddModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { }, "", true);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/ContactAdd.cshtml", model);
        }

        [HttpPost]
        public JsonResult AddContact(ContactInfoModel contact)
        {
            try
            {
                if (IsContactsAdmin())
                {
                    ModelState.Clear();
                    var contactSimpleInfo = _contactProvider.SaveNewContact(contact);
                    return Json("{\"success\": true,\"id\":" + contactSimpleInfo.ID + ",\"type\":\"" + contact.Type.GetDisplay() + "\"}");
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Create Contact\",\"message\" : \"No " + contact.Type.GetDisplay() + " has been created.Contact your administrator\"}}");
            }
        }

        private ActionResult ContactsEnumeration_GridCoreAction(int ID, GridViewModel gridViewModel, string selectFolderName, string search = "",
            TypeContextMenu typeContextMenu = TypeContextMenu.Block, string xml = null, bool ShowSelectedContactsGrid = false, string SelectedValues = "", eContactType type = eContactType.None)
        {
            ContactsEnumerationModel model = new ContactsEnumerationModel(_settings, _flexpageProcessor, Request.UrlReferrer != null ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query) : null);
            if (!string.IsNullOrEmpty(xml))
            {
                HttpCookie cookie = Request.Cookies[xml];
                xml = cookie.Value;
            }
            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, false, selectFolderName, typeContextMenu, _contactProvider, type, xml, ShowSelectedContactsGrid, SelectedValues);
            model.Search = search;
            model.GridModel = gridViewModel;
            model.GridModel.ProcessCustomBinding(
                model.GetDataObjectsCount,
                model.GetDataObjects
            );
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactsEnumeration/_ContactsEnumerationGrid.cshtml", model);
        }

        private ContactsEnumerationModel ContactsEnumerationModel(int ID, ContactsEnumerationModel model = null, bool ProcessCustomBinding = true, string selectFolderName = null, TypeContextMenu typeContextMenu = TypeContextMenu.Block,
            string xml = null, int? contactID = null, int? contactShortcutID = null, eContactType ShowContacts = eContactType.None, bool ShowSelectedContactsGrid = false, string SelectedValues = "")
        {
            if (model == null)
            {
                model = new ContactsEnumerationModel(_settings, _flexpageProcessor, Request.UrlReferrer != null ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query) : null);
            }
            if (!string.IsNullOrEmpty(xml))
            {
                HttpCookie cookie = Request.Cookies[xml];
                xml = cookie.Value;
            }
            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, selectFolderName, typeContextMenu, _contactProvider, ShowContacts, xml, ShowSelectedContactsGrid, SelectedValues);
            if (ProcessCustomBinding)
            {
                model.GridModel.ProcessCustomBinding(
                    new GridViewCustomBindingGetDataRowCountHandler(args => { model.GetDataObjectsCount(args, contactID, contactShortcutID, ShowContacts); }),
                    new GridViewCustomBindingGetDataHandler(args => { model.GetDataObjects(args, contactID, contactShortcutID, ShowContacts); })
                );
            }
            return model;
        }

        private bool IsContactsAdmin()
        {
            return _permissionsService.IsContactsAdmin(User.Identity.Name);
        }
    }
}