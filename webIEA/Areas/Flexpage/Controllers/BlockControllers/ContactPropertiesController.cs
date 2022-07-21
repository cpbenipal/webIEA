    using Flexpage.Abstract;
using Pluritech.Permissions.Abstract;
using Pluritech.Properties.Abstract;
using Pluritech.Properties.Abstract.DTO;
using Pluritech.Settings.Abstract;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class ContactPropertiesController : Controller
    {
        protected readonly IPropertyProvider _propertyService;
        protected readonly ILocalization _localization;
        protected readonly IPermissionsService _permissionsService;
        protected readonly IFlexpageSettings _settings;

        public ContactPropertiesController(IPropertyProvider propertyService, ILocalization localization, IPermissionsService permissionsService,
            IFlexpageSettings settings)
        {
            _propertyService = propertyService;
            _localization = localization;
            _permissionsService = permissionsService;
            _settings = settings;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_CustomPropertyAdd(CustomPropertyInfo viewModel, int contactID, int? linkedBlockID, bool allowEdit)
        {
            CustomPropertyModel model = new CustomPropertyModel();
            try
            {
                if (IsContactsAdmin())
                {
                    _propertyService.SaveObjectCustomPropertiesValues(new CustomPropertyModel()
                    {
                        ObjectID = contactID,
                        Properties = new List<CustomPropertyInfo>() { viewModel }
                    });
                    model = _propertyService.GetObjectCustomPropertiesModel(contactID);
                    model.AllowEdit = allowEdit;
                    model.LinkedBlockID = linkedBlockID;
                    model.LangCode = String.IsNullOrEmpty(model.LangCode) ? _localization.GetCurrentOrDefaultLangCode() : model.LangCode;
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
            ViewBag.IsAdmin = IsAdmin();
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_CustomPropertiesGrid.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_CustomPropertyEdit(CustomPropertyInfo viewModel, int contactID, int? linkedBlockID, bool allowEdit)
        {
            CustomPropertyModel model = new CustomPropertyModel();
            try
            {
                if (IsContactsAdmin())
                {
                    _propertyService.SaveObjectCustomPropertiesValues(new CustomPropertyModel()
                    {
                        ObjectID = contactID,
                        Properties = new List<CustomPropertyInfo>() { viewModel }
                    });
                    model = _propertyService.GetObjectCustomPropertiesModel(contactID);
                    model.AllowEdit = allowEdit;
                    model.LinkedBlockID = linkedBlockID;
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
            ViewBag.IsAdmin = IsAdmin();
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_CustomPropertiesGrid.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ContactDetails_CustomPropertyDelete(int ID, int contactID, int? linkedBlockID, bool allowEdit)
        {
            CustomPropertyModel model = new CustomPropertyModel();
            try
            {
                if (IsContactsAdmin())
                {
                    _propertyService.SaveObjectCustomPropertiesValues(new CustomPropertyModel()
                    {
                        ObjectID = contactID,
                        ToDelete = new List<int> { ID }
                    });
                    model = _propertyService.GetObjectCustomPropertiesModel(contactID);
                    model.AllowEdit = allowEdit;
                    model.LinkedBlockID = linkedBlockID;
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
            ViewBag.IsAdmin = IsAdmin();
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_CustomPropertiesGrid.cshtml", model);
        }

        public ActionResult ContactDetails_CustomPropertyUpdate(CustomPropertyInfo viewModel, int contactID, int? linkedBlockID, bool allowEdit)
        {
            CustomPropertyModel model = new CustomPropertyModel();
            try
            {
                model = _propertyService.GetObjectCustomPropertiesModel(contactID);
                model.AllowEdit = allowEdit;
                model.LinkedBlockID = linkedBlockID;
                model.LangCode = String.IsNullOrEmpty(model.LangCode) ? _localization.GetCurrentOrDefaultLangCode() : model.LangCode;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.IsAdmin = IsAdmin();
            ViewBag.IsContactsAdmin = IsContactsAdmin();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_CustomPropertiesGrid.cshtml", model);
        }

        protected bool IsAdmin()
        {
            return _settings.IsCmsAdmin();
        }

        protected bool IsContactsAdmin()
        {
            return _permissionsService.IsContactsAdmin(User.Identity.Name);
        }
    }
}