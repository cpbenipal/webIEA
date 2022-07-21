using DevExpress.Utils.Extensions;
using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Search;
using Flexpage.Models;
using Flexpage.Service.Abstract;
using Pluritech.Authentication.Abstract;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.Permissions.Abstract;
using Pluritech.Permissions.Abstract.DTO;
using Pluritech.Properties.Abstract;
using Pluritech.Properties.Abstract.DTO;
using Pluritech.Shared.Abstract.DTO;
using Pluritech.UserProfile.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class ContactDetailsController : Controller
    {
        private readonly IFlexpageRepository _repository;
        private readonly IFlexpageSettings _settings;
        private readonly IFlexpage _flexpageProcessor;
        private readonly IContactProvider _contactProvider;
        protected readonly IPermissionsService _permissionsService;
        protected readonly IPropertyProvider _propertyService;
        private readonly IUserGeneralPermissionProvider _userGeneralPermissionProvider;
        private readonly IUser _user;
        private readonly ISettingsProvider _websiteSettings;

        public ContactDetailsController(IFlexpageRepository repository, IFlexpageSettings settings, IFlexpage flexpageProcessor,
            IContactProvider contactProvider, IPermissionsService permissionsService, IPropertyProvider propertyService,
            IUserGeneralPermissionProvider userGeneralPermissionProvider, IUser user, ISettingsProvider websiteSettings)
        {
            _flexpageProcessor = flexpageProcessor;
            _repository = repository;
            _settings = settings;
            _contactProvider = contactProvider;
            _permissionsService = permissionsService;
            _propertyService = propertyService;
            _userGeneralPermissionProvider = userGeneralPermissionProvider;
            _user = user;
            _websiteSettings = websiteSettings;
        }

        public ActionResult ContactDetails(int ID, int contactID, eContactType contactType, int? selectTabIndex = 0, string selectTab = "GeneralInfo")
        {
            var model = GenerateContactDetailsModel(ID, contactID, contactType, selectTabIndex, selectTab, CanEditPassword(contactID));
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_ContactDetails.cshtml", model);
        }

        public PartialViewResult ContactDetailsPageControl(int ID, int contactID, eContactType contactType, int? selectTabIndex = 0, string selectTab = "GeneralInfo")
        {
            var model = GenerateContactDetailsModel(ID, contactID, contactType, selectTabIndex, selectTab);
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_ContactDetailsPageControl.cshtml", model);
        }

        public ActionResult ContactDetailsEmpty(int ID)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, "", true);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_ContactDetails.cshtml", model);
        }
        public ActionResult ContactDetails_UpdateTab(int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = GenerateContactDetailsModel(blockID, contactID, contactType, selectTabIndex, selectTab, edit && IsContactsAdmin());
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        public ActionResult ContactDetails_UpdateForm(int ID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = GenerateContactDetailsModel(ID, contactID, contactType, selectTabIndex, selectTab, edit && IsContactsAdmin());
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_Form.cshtml", model);
        }

        [HttpPost]
        public ActionResult ContactDetails_SaveGeneralInfoPerson(int ID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, PersonViewModel newPersonModel = null)
        {
            var edit = false;
            var valid = ModelState.All(m => m.Value.Errors.Count == 0);
            if (!valid)
            {
                edit = true;
            }
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);

            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            if (selectTab == "LinkedPersons")
            {
                if (contactType == eContactType.Person)
                {
                    model.LinkedPersons.AddRange(_contactProvider.GetLinkedPersons(model.ObjectID));
                }
                else if (contactType == eContactType.Company)
                {
                    model.LinkedPersons.AddRange(_contactProvider.GetLinkedPersons(null, model.ObjectID));
                }
            }
            newPersonModel.Notes = newPersonModel.Notes ?? "";
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            if (selectTabIndex == 0 && newPersonModel != null && newPersonModel.Type == eContactType.Person)
            {
                model.AssignGeneralInfo(newPersonModel);
                if (valid)
                {
                    try
                    {
                        if (IsContactsAdmin())
                        {
                            //model.ApplyGeneralInfo(_repository);
                            newPersonModel.ShortcutID = model.ObjectID;
                            _contactProvider.SaveGeneralInfoPerson(newPersonModel);
                        }
                        else
                        {
                            throw new Exception("Not enough permissions");
                        }

                    }
                    catch
                    {
                        edit = true;
                        model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
                        model.AssignGeneralInfo(newPersonModel);
                    }
                }
                else
                {
                    edit = true;
                    model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
                    model.AssignGeneralInfo(newPersonModel);
                    ViewData["EditError"] = "Model no valid";
                }
            }

            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }

        [HttpPost]
        public ActionResult ContactDetails_SaveGeneralInfoCompany(int ID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, CompanyViewModel newCompanyModel = null)
        {
            var edit = false;
            var valid = ModelState.All(m => m.Value.Errors.Count == 0);
            if (!valid)
            {
                edit = true;
            }
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);

            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            newCompanyModel.Notes = newCompanyModel.Notes ?? "";
            if (selectTabIndex == 0 && newCompanyModel != null && newCompanyModel.Type == eContactType.Company)
            {
                model.AssignGeneralInfo(newCompanyModel);
                if (valid)
                {
                    try
                    {
                        if (IsContactsAdmin())
                        {
                            //model.ApplyGeneralInfo(_repository);
                            newCompanyModel.ShortcutID = model.ObjectID;
                            _contactProvider.SaveGeneralInfoCompany(newCompanyModel);
                        }
                        else
                        {
                            throw new Exception("Not enough permissions");
                        }
                    }
                    catch
                    {
                        edit = true;
                        model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
                        model.AssignGeneralInfo(newCompanyModel);
                    }

                }
                else
                {
                    edit = true;
                    model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
                    model.AssignGeneralInfo(newCompanyModel);
                    ViewData["EditError"] = "Model no valid";
                }
            }

            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }

        [HttpPost]
        public ActionResult ContactDetails_SaveAdministration(int ID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, ContactDetailsModel newModel = null)
        {
            var edit = false;
            var valid = ModelState.Where(m => m.Key.Contains("Administration") || m.Key.Contains("Credentials")).All(m => m.Value.Errors.Count == 0);
            if (!valid)
            {
                edit = true;
            }
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);

            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);

            if (selectTab == "Administration" || selectTab == "Credentials")
            {
                var oldLogin = model.Administration.Login;
                model.AssignAdministration(newModel.Administration);
                if (_contactProvider.ExistWebLogin(newModel.Administration.Login) && oldLogin != newModel.Administration.Login)
                {
                    edit = true;
                    model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
                    model.AssignAdministration(newModel.Administration);
                    ViewData["EditError"] = "This login already exists";
                }
                else if (valid)
                {
                    try
                    {
                        if (IsContactsAdmin())
                        {
                            if (model.ContactType == eContactType.Person)
                            {
                                _contactProvider.SaveWebLogin(model.Person.ID, newModel.Administration.Login, newModel.Administration.Password);
                            }
                            else
                            {
                                throw new Exception("Type should be Person");
                            }
                        }
                        else
                        {
                            throw new Exception("Not enough permissions");
                        }
                    }
                    catch (Exception e)
                    {
                        edit = true;
                        model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
                        model.AssignAdministration(newModel.Administration);
                        ViewData["EditError"] = e.Message;
                    }
                }
                else
                {
                    edit = true;
                    model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
                    model.AssignAdministration(newModel.Administration);
                    ViewData["EditError"] = "The model is not valid";
                }
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }

        #region Address
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_AddressAdd(AddressView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            ModelState.Clear();
            viewModel = ContactDetails_GetCustomProperties(viewModel);
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            try
            {
                if (IsContactsAdmin())
                {
                    viewModel.ID =
                        _contactProvider.AddContactAddress(viewModel, contactType, contactID, model.ObjectID);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            finally
            {
                model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID,
                    contactType, edit, (int)selectTabIndex, selectTab);
                model.Address = model.Address.OrderBy(t => t.ID).ToList();
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_AddressEdit(AddressView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            ModelState.Clear();
            viewModel = ContactDetails_GetCustomProperties(viewModel);
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            try
            {
                if (IsContactsAdmin())
                {
                    viewModel.ID = _contactProvider.EditContactAddress(viewModel, contactType, contactID, model.ObjectID);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            finally
            {
                model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID,
                    contactType, edit, (int)selectTabIndex, selectTab);
                model.Address = model.Address.OrderBy(t => t.ID).ToList();
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_AddressDelete(int ID, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            ModelState.Clear();
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            try
            {
                if (IsContactsAdmin())
                {
                    _contactProvider.DeleteContactAddress(ID, contactType, contactID, model.ObjectID);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            finally
            {
                model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID,
                    contactType, edit, (int)selectTabIndex, selectTab);
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        #endregion

        #region Telecom
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_TelecomsAdd(TelecomView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    viewModel.ID = _contactProvider.AddContactTelecom(viewModel, contactType, contactID, model.ObjectID);
                    // only one telecom of a type can be default
                    if (viewModel.IsDefault == true)
                    {
                        model.Telecoms.Where(t => t.TypeID == viewModel.TypeID).ForEach(t => t.IsDefault = false);
                    }
                    // if no telecoms are default, the new one is automatically forced as default
                    else if (model.Telecoms.Where(t => t.TypeID == viewModel.TypeID).All(t => t.IsDefault != true))
                    {
                        viewModel.IsDefault = true;
                    }
                    model.Telecoms.Add(viewModel);
                    model.Telecoms = model.Telecoms.OrderBy(t => t.ID).OrderByDescending(a => a.IsDefault).ToList();
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_TelecomsEdit(TelecomView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    model.Telecoms.RemoveAll(t => t.ID == viewModel.ID);
                    // only one telecom of a type can be default
                    if (viewModel.IsDefault == true)
                    {
                        model.Telecoms.Where(t => t.TypeID == viewModel.TypeID).ForEach(t => t.IsDefault = false);
                    }
                    // if no telecoms are default, the new one is automatically forced as default
                    else if (model.Telecoms.Where(t => t.TypeID == viewModel.TypeID).All(t => t.IsDefault != true))
                    {
                        viewModel.IsDefault = true;
                    }
                    viewModel.ID = _contactProvider.EditContactTelecom(viewModel, contactType, contactID, model.ObjectID);
                    model.Telecoms.Add(viewModel);
                    model.Telecoms = model.Telecoms.OrderBy(t => t.ID).OrderByDescending(a => a.IsDefault).ToList();
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_TelecomsDelete(int ID, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    _contactProvider.DeleteContactTelecom(ID, contactType, contactID, model.ObjectID);
                    model.Telecoms.RemoveAll(a => a.ID == ID);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        #endregion

        #region BankAccounts
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_BankAccountsAdd(BankAccountView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    model.AddBankAccount(viewModel);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_BankAccountsEdit(BankAccountView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    model.EditBankAccount(viewModel);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_BankAccountsDelete(int ID, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    model.DeleteBankAccount(ID);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        #endregion

        #region LinkedPerson
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_LinkedPersonAdd(LinkedView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            viewModel = ContactDetails_GetCustomProperties(viewModel);
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            if (selectTab == "LinkedPersons")
            {
                model.LinkedPersons.AddRange(LoadLinkedPersons(model.ObjectID, contactType));
            }
            try
            {
                if (IsContactsAdmin())
                {
                    model.CheckLinked(viewModel);
                    model.GetLinkedObjectShortcut(viewModel, eContactType.Person);
                    viewModel = _contactProvider.AddLinkedPerson(viewModel, contactType, contactID, model.ObjectID);
                    model.AddLinkedPerson(viewModel);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_LinkedPersonEdit(LinkedView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            viewModel = ContactDetails_GetCustomProperties(viewModel);
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            if (selectTab == "LinkedPersons")
            {
                model.LinkedPersons.AddRange(LoadLinkedPersons(model.ObjectID, contactType));
            }
            try
            {
                if (IsContactsAdmin())
                {
                    model.CheckLinked(viewModel);
                    model.GetLinkedObjectShortcut(viewModel, eContactType.Person);
                    var oldID = viewModel.ID;
                    viewModel = _contactProvider.EditLinkedPerson(viewModel, contactType, contactID, model.ObjectID);
                    model.EditLinkedPerson(oldID, viewModel);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_LinkedPersonDelete(int ID, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            if (selectTab == "LinkedPersons")
            {
                model.LinkedPersons.AddRange(LoadLinkedPersons(model.ObjectID, contactType));
            }
            try
            {
                if (IsContactsAdmin())
                {
                    _contactProvider.DeleteLinkedPerson(ID, contactType, contactID, model.ObjectID);
                    model.DeleteLinkedPerson(ID);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }

        private IEnumerable<LinkedView> LoadLinkedPersons(int contactID, eContactType contactType)
        {
            if (contactType == eContactType.Person)
            {
                return _contactProvider.GetLinkedPersons(contactID);
            }
            else if (contactType == eContactType.Company)
            {
                return _contactProvider.GetLinkedPersons(null, contactID);
            }

            return null;
        }

        #endregion

        #region LinkedCompany
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_LinkedCompanyAdd(LinkedView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            viewModel = ContactDetails_GetCustomProperties(viewModel);
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    model.CheckLinked(viewModel);
                    model.GetLinkedObjectShortcut(viewModel, eContactType.Company);
                    viewModel = _contactProvider.AddLinkedCompany(viewModel, contactType, contactID, model.ObjectID);
                    model.AddLinkedCompany(viewModel);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_LinkedCompanyEdit(LinkedView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            viewModel = ContactDetails_GetCustomProperties(viewModel);
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    model.CheckLinked(viewModel);
                    model.GetLinkedObjectShortcut(viewModel, eContactType.Company);
                    var oldID = viewModel.ID;
                    viewModel = _contactProvider.EditLinkedCompany(viewModel, contactType, contactID, model.ObjectID);
                    model.EditLinkedCompany(oldID, viewModel);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_LinkedCompanyDelete(int ID, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    _contactProvider.DeleteLinkedCompany(ID, contactType, contactID, model.ObjectID);
                    model.DeleteLinkedCompany(ID);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }

        public LinkedView ContactDetails_GetCustomProperties(LinkedView model)
        {
            var objectProperties = _repository.GetObjectProperties().Where(op => op.ObjectType.Any(ot => ot.ID == (int)Pluritech.Properties.Abstract.ObjectType.ObjectLink));
            model.CustomProperties = new List<CustomPropertyInfo>();
            foreach (var property in objectProperties)
            {
                var value = Request.Params[property.Name];
                if (value != null)
                {
                    model.CustomProperties.Add(new CustomPropertyInfo()
                    {
                        Value = value,
                        Type = (Pluritech.Properties.Abstract.DTO.ObjectPropertyType)property.TypeID,
                        Name = property.Name
                    });
                }
            }
            return model;
        }

        public AddressView ContactDetails_GetCustomProperties(AddressView model)
        {
            var objectProperties = _repository.GetObjectProperties().Where(op => op.ObjectType.Any(ot => ot.ID == (int)Pluritech.Properties.Abstract.ObjectType.ObjectLink));
            model.CustomProperties = new List<CustomPropertyInfo>();
            foreach (var property in objectProperties)
            {
                var value = Request.Params[property.Name];
                if (value != null)
                {
                    model.CustomProperties.Add(new CustomPropertyInfo()
                    {
                        Value = value,
                        Type = (Pluritech.Properties.Abstract.DTO.ObjectPropertyType)property.TypeID,
                        Name = property.Name
                    });
                }
            }
            return model;
        }
        #endregion

        #region LinkedFolders
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_LinkedFoldersAdd(LinkedFolderView viewModel, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    if (model.Company != null)
                    {
                        _contactProvider.AddCompanyToFolder(model.Company.ID, viewModel.ID);
                    }
                    if (model.Person != null)
                    {
                        _contactProvider.AddPersonToFolder(model.Person.ID, viewModel.ID);
                    }
                    model.AddLinkedFolders(viewModel);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_LinkedFoldersDelete(int ID, int blockID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = blockID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab);
            try
            {
                if (IsContactsAdmin())
                {
                    if (model.Company != null)
                    {
                        _contactProvider.DeleteCompanyFromFolder(model.Company.ShortcutID.Value, ID);
                    }
                    if (model.Person != null)
                    {
                        _contactProvider.DeletePersonFromFolder(model.Person.ShortcutID.Value, ID);
                    }
                    model.DeleteLinkedFolders(ID);
                }
                else
                {
                    throw new Exception("Not enough permissions");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_" + selectTab + ".cshtml", model);
        }

        #endregion
        [ValidateInput(false)]
        public ActionResult ContactDetails_GetCompanies(string textField, string valueField, int ContactID, eContactType contactType)
        {
            var Companies = new List<LinkedViewName>();
            var keys = Request.Form.AllKeys;
            var values = Request.Form.Get(keys[4]).Replace(",", "").Split(' ').ToList();
            List<int> search = new List<int>();
            values.ForEach(value =>
            {
                search.AddRange(SearchPropertyQuery.SearchProperties(SearchPropertyTypeType.Company,
                 new FilterDesciption() { Field = "Search", Value = value, FilterMode = eFilterMode.Contains }).Select(s => s.ObjectID).ToList());
            });
            var filterBy = new List<FilterDesciption>();
            var contacts = search.ToList();
            filterBy.Add(new FilterDesciption()
            {
                Field = eContactType.Company.ToString(),
                Table = "SearchResultShortcut",
                Value = contacts
            });
            _repository.QueryContacts.FilterBy(filterBy).ForEach(c =>
            {
                var id = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.ID")?.Value;
                var firstName = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.FirstName")?.Value;
                var lastName = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.LastName")?.Value;
                var voidF = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.Void")?.Value;
                if ((ContactID != (int)id || contactType == eContactType.Person) && (bool)voidF == false)
                {
                    Companies.Add(new LinkedViewName()
                    {
                        LinkID = (int)id,
                        LinkShortcutID = (int)c.ID,
                        LinkName = firstName + " " + lastName
                    });
                }
            });
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ValueField = valueField;
                p.TextField = textField;
                p.BindList(Companies);
            });
        }

        [ValidateInput(false)]
        public ActionResult ContactDetails_GetPersons(string textField, string valueField, int ContactID, eContactType contactType)
        {
            var Persons = new List<LinkedViewName>();

            var keys = Request.Form.AllKeys;
            var values = Request.Form.Get(keys[4]).Replace(",", "").Split(' ').ToList();
            List<int> search = new List<int>();
            values.ForEach(value =>
            {
                search.AddRange(SearchPropertyQuery.SearchProperties(SearchPropertyTypeType.Person,
                 new FilterDesciption() { Field = "Search", Value = value, FilterMode = eFilterMode.Contains }).Select(s => s.ObjectID).ToList());
            });
            var filterBy = new List<FilterDesciption>();
            var contacts = search.ToList();
            filterBy.Add(new FilterDesciption()
            {
                Field = eContactType.Person.ToString(),
                Table = "SearchResultShortcut",
                Value = contacts
            });
            _repository.QueryContacts.FilterBy(filterBy).ForEach(c =>
            {
                var id = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.ID")?.Value;
                var firstName = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.FirstName")?.Value;
                var lastName = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.LastName")?.Value;
                var voidF = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.Void")?.Value;
                if ((ContactID != (int)id || contactType == eContactType.Company) && (bool)voidF == false)
                {
                    Persons.Add(new LinkedViewName()
                    {
                        LinkID = (int)id,
                        LinkShortcutID = (int)c.ID,
                        LinkName = firstName + " " + lastName
                    });
                }
            });

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ValueField = valueField;
                p.TextField = textField;
                p.BindList(Persons);
            });
        }
        [ValidateInput(false)]
        public ActionResult ContactDetails_GetLinkTypesCompanies(string companyName, string textField, string valueField)
        {
            var LinkTypesCompanies = _repository.GetLinkTypes().Where(lt => lt.ParentTypeID == (int)ObjectTypeEnum.CompanyObject).ToList();
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ValueField = valueField;
                p.TextField = textField;
                p.BindList(LinkTypesCompanies);
            });
        }
        [ValidateInput(false)]
        public ActionResult ContactDetails_GetLinkTypesPersons(string companyName, string textField, string valueField)
        {
            var LinkTypesPersons = _repository.GetLinkTypes().Where(lt => lt.ParentTypeID == (int)ObjectTypeEnum.PersonObject).ToList();

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ValueField = valueField;
                p.TextField = textField;
                p.BindList(LinkTypesPersons);
            });
        }

        private ContactDetailsModel GenerateContactDetailsModel(int ID, int contactID, eContactType contactType, int? selectTabIndex, string selectTab, bool edit = false)
        {
            var model = new ContactDetailsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, true, contactID, contactType, edit, (int)selectTabIndex, selectTab, CanEditPassword(contactID));
            switch (selectTab.ToLower())
            {
                case "customproperties":
                    model.CustomProperties = _propertyService.GetObjectCustomPropertiesModel(model.ObjectID);
                    model.CustomProperties.AllowEdit = model.AllowEdit && IsContactsAdmin();
                    model.CustomProperties.LinkedBlockID = model.ID;
                    break;
                case "linkedpersons":
                    model.LinkedPersons.AddRange(LoadLinkedPersons(model.ObjectID, contactType));
                    model.AllowedPropertiesObjectLink = _propertyService.EnumerateProperties(Pluritech.Properties.Abstract.ObjectType.ObjectLink);
                    break;
            }
            model.AllowedPropertiesObjectLink = _propertyService.EnumerateProperties(Pluritech.Properties.Abstract.ObjectType.ObjectLink);
            model.Columns.ForEach(c =>
            {
                var prop = _propertyService.EnumerateAllProperties().FirstOrDefault(p => p.Name == c.Name && p.EnumID != null);
                c.AllowedValues = prop != default(Pluritech.Properties.Abstract.DTO.Property) ? prop.EnumValues : null;
            });
            ViewBag.IsAdmin = model.AdminMode;
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return model;
        }

        protected bool IsPWAdmin()
        {
            return _userGeneralPermissionProvider.IsPWAdmin();
        }

        private bool IsContactsAdmin()
        {
            return _permissionsService.IsContactsAdmin(User.Identity.Name);
        }

        /// <summary>
        /// Checks whether current user can edit selected contact's password
        /// </summary>
        /// <param name="contactID">Target contact's ID</param>
        /// <returns></returns>
        private bool CanEditPassword(int contactID)
        {
            // PW admins and users with general permission can edit passwords 
            if (IsPWAdmin() || _permissionsService.UserHasGeneralPermission(eGeneralPermission.EditWebPasswords))
                return true;
            // All users can edit their own pasword if website setting is enabled
            if (_user.ID == contactID)
            {
                bool r;
                bool.TryParse(_websiteSettings.GetSettings(WebsiteSettingsNames.UsersCanEditOwnPassword), out r);
                return r;
            }

            return false;
        }


    }
}