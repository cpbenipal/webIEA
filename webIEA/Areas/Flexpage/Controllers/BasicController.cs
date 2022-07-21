using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Models;
using Pluritech.Settings.Abstract;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Pluritech.Permissions.Abstract;
using Pluritech.Properties.Abstract;
using Pluritech.Contact.Abstract;
using Pluritech.Permissions.Abstract.DTO;
using Pluritech.UserProfile.Abstract;

namespace Flexpage.Controllers
{
    /// <summary>
    /// This is a common ancestor of all Flexpage controllers.
    /// </summary>
    public abstract class BasicController : LocalizedController
    {
        protected const string EditorTemplatePath = "~/Areas/Flexpage/Views/Flexpage/Editors/{0}.cshtml";
        private const string NoSettingsViewPath = "~/Areas/Flexpage/Views/Flexpage/Editors/Default/NoSettings.cshtml";
        protected readonly IFlexpageSettings _settings;
        protected readonly IFlexpageRepository _repository;
        protected readonly IFlexpage _flexpageProcessor;

        private string _tempUploadedImagesPath;
        protected readonly IPermissionsService _permissionsService;
        protected readonly IPropertyProvider _propertyService;
        protected readonly IContactProvider _contactProvider;
        private readonly IUserGeneralPermissionProvider _userGeneralPermissionProvider;

        public BasicController(IFlexpageRepository repository, IFlexpageSettings settings, ILocalization localization, IPermissionsService permService
            , IPropertyProvider propertyService, IContactProvider contactProvider, IFlexpage flexpageProcessor, IUserGeneralPermissionProvider userGeneralPermissionProvider)
            :base(settings, localization)
        {
            _settings = settings;
            _repository = repository;
            _permissionsService = permService;
            _propertyService = propertyService;
            _contactProvider = contactProvider;
            _flexpageProcessor = flexpageProcessor;
            _tempUploadedImagesPath = WebConfigurationManager.AppSettings["TempUploadedImagesPath"];
            _userGeneralPermissionProvider = userGeneralPermissionProvider;
        }

        public bool IsPartialViewExists(string name)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(this.ControllerContext, name);
            return (result.View != null);
        }

        protected bool IsAdmin()
        {
            return _settings.IsCmsAdmin();
        }

        protected bool IsContactsAdmin()
        {
            return _permissionsService.IsContactsAdmin(User.Identity.Name);
        }

        protected bool IsPWAdmin()
        {
            return _userGeneralPermissionProvider.IsPWAdmin();
        }

        [HttpPost]
        [Route("{alias}")]
        public JsonResult UploadFiles(string alias)
        {
            FileUploadModel uploadModel = new FileUploadModel();
            try
            {
                string directoryPath = Server.MapPath("~" + _tempUploadedImagesPath + alias);
                if(Directory.Exists(directoryPath) == false)
                {
                    Directory.CreateDirectory(directoryPath);
                }
                foreach(string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if(fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        var path = Path.Combine(directoryPath, fileContent.FileName);
                        using(var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        uploadModel.FilesData.Add(new FileTuple
                        {
                            FileName = fileContent.FileName,
                            FilePath = _tempUploadedImagesPath + (string.IsNullOrEmpty(alias) ? "" : alias + "/") + fileContent.FileName
                        });
                    }
                    uploadModel.Message = "Success";
                }

                uploadModel.StatusCode = "200";
                if(Request.Files.Count == 0)
                {
                    uploadModel.Message = "NoFilesFound";
                }
                return Json(uploadModel);
            }
            catch
            {
                uploadModel.StatusCode = ((int)HttpStatusCode.BadRequest).ToString();
                uploadModel.Message = "Error";
                return Json(uploadModel);
            }
        }
        public virtual PartialViewResult GetBlockSettingsView(BlockCommandModel model)
        {
            if (IsAdmin() || IsContactsAdmin())
            {
                ViewModel m = CreateAndLoadModel(model, true, model.Title);
                    
                return GetEditorFor(m);
            }

            if (model.BlockType == "CustomPropertyAdd" &&
                _permissionsService.UserHasGeneralPermission(eGeneralPermission.ManageFolderProperties))
            {
                ViewModel m = CreateAndLoadModel(model, true, model.Title);

                return GetEditorFor(m);
            }
            throw new Exception("Not enough permissions");
        }
        [HttpPost]
        public PartialViewResult GetCustomPopupContent(BlockCommandModel model)
        {
            if (model.Command == "addContact"&&IsContactsAdmin())
            {
                return PartialView("~/Areas/Flexpage/Views/Shared/PopupEditContent.cshtml", model);
            }
            if ( model.Command == "choose")
            {
                return PartialView("~/Areas/Flexpage/Views/Shared/PopupChooseContent.cshtml", model);
            }
            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                return PartialView("~/Areas/Flexpage/Views/Shared/PopupUrlContent.cshtml", model.Url);
            }
            if (model.Command == "notification")
            {
                return PartialView("~/Areas/Flexpage/Views/Shared/PopupEditContent.cshtml", model);
            }
            if(!string.IsNullOrWhiteSpace(model.Action))
            {
                model.Controller = string.IsNullOrWhiteSpace(model.Controller) ? "Flexpage" : model.Controller;
                return PartialView("~/Areas/Flexpage/Views/Shared/PopupCustomContent.cshtml", model);
            }

            if (!string.IsNullOrEmpty(model.BlockType))
            {
                Abstract.DTO.IBlockModel blockModel = _flexpageProcessor.LoadBlockEditorModel(model.BlocklistID, model.ID, model.BlockType, model.BlockAlias,model.BlockAfter,
                    Request.QueryString);
                if(blockModel != null)
                {
                    return GetEditorForBlock(blockModel);
                }

                if (model.BlockType == "ObjectProperties")
                {
                    // Users with permission should get the edit version of the popup, otherwise they can only view it
                    if (!_permissionsService.UserHasGeneralPermission(eGeneralPermission.ManageFolderProperties))
                        return GetObjectPropertiesView(model);

                    model.Action = "GetObjectPropertiesView";
                    model.Controller = "Flexpage";
                    model.Parameters = "CanEdit";
                    return PartialView("~/Areas/Flexpage/Views/Shared/PopupEditContent.cshtml", model);
                }

                if (model.BlockType == "CustomPropertyAdd")
                {
                    model.Controller = "Flexpage";
                    return PartialView("~/Areas/Flexpage/Views/Shared/PopupEditContent.cshtml", model);
                }

                ViewModel m = CreateAndLoadModel(model, true, model.Title);
                return GetEditorFor(m);
            }
            return default(PartialViewResult);
        }

        public PartialViewResult GetObjectPropertiesView(BlockCommandModel model)
        {
            var m = (ObjectPropertiesModel)CreateAndLoadModel(model, true, model.Title);
            m.AllowCustomPropertiesEdit = model.Parameters == "CanEdit";
            m.IsEdit = _settings.IsCmsAdmin();
            m.Properties = _propertyService.GetObjectCustomPropertiesModel(m.ID);
            m.Permissions = _permissionsService.GetObjectPermissions(m.ID);
            m.Publication = _contactProvider.GetFileShortcutPublications(m.ID);

            return GetEditorFor(m);
        }

        /// <summary>
        /// First, extracts model class name from field named "ViewModel" in provided form.
        /// Second, creates model instance whith that name.
        /// Third, loads its content from provided repository's entity
        /// </summary>
        /// <param name="form">Provided form</param>
        /// <returns>An instance of ViewModel-dervied class, filled with repository's entity contents</returns>
        protected ViewModel CreateAndLoadModel(BlockCommandModel proto, bool needToLoadContent = true, string title="" )
        {
            string mcn = proto.BlockType.EndsWith("Model") ? proto.BlockType : proto.BlockType + "Model";
            ViewModel m = ViewModel.Create(mcn, _settings, _flexpageProcessor);
            m.Load(_repository, proto,title);
            m.EditorPostfix = proto.IDPostfix;
            return m;
        }

        //protected ViewModel CreateAndLoadModel(BlockCommandModel proto, IFlexpageRepository repository, IFlexpage flexpageProcessor, bool needToLoadContent = true, string title = "")
        //{
        //    string mcn = proto.BlockType.EndsWith("Model") ? proto.BlockType : proto.BlockType + "Model";
        //    ViewModel m = ViewModel.Create(mcn, _settings, flexpageProcessor);
        //    m.Load(repository, proto, title);
        //    m.EditorPostfix = proto.IDPostfix;
        //    return m;
        //}

        protected PartialViewResult GetEditorForBlock(Abstract.DTO.IBlockModel target)
        {
            string editorPath;
            if (!string.IsNullOrEmpty(target.EditorViewPath))
            {
                editorPath = target.EditorViewPath;
            }
            else
            {
                string s = target.BlockType ?? target.GetType().Name;
                if (s.EndsWith("Model"))
                {
                    s = s.Remove(s.Length - 5);
                }
                editorPath = string.Format(EditorTemplatePath, s);
            }
            return PartialView(editorPath, target);
        }

        protected ActionResult GetInvalidModelResponse(ModelStateDictionary ModelState)
        { //returns a response with validation errors to be displayed on a popup
            var errorModel =
                from x in ModelState.Keys
                where ModelState[x].Errors.Count > 0
                select new
                {
                    key = x,
                    errors = ModelState[x].Errors.
                                                  Select(y => y.ErrorMessage).
                                                  ToArray()
                };
            HttpContext.Response.StatusCode = 422;
            return Json(errorModel);
        }



        protected PartialViewResult GetEditorFor(ViewModel target)
        {
            string s = target.GetType().Name;
            if(s.EndsWith("Model"))
                s = s.Remove(s.Length - 5);

            target.FillViewData(ViewData, _repository);

            try
            {
                ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, string.Format(EditorTemplatePath, s), null);
                if (result.View != null)
                {
                    return PartialView(string.Format(EditorTemplatePath, s), target);
                }
                return PartialView(NoSettingsViewPath, target);
            }
            catch
            {
                return PartialView(NoSettingsViewPath);
            }
        }
    }
}