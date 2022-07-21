using DevExpress.Web;
using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Code.Helpers;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Flexpage.Models;
using FlexPage2.Areas.Flexpage.Infrastructure;
using Newtonsoft.Json;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.FileSystem.Abstract;
using Pluritech.Settings.Abstract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Pluritech.Permissions.Abstract;
using Pluritech.Properties.Abstract;

using System.Web;
using Pluritech.Pluriworks.Service.Abstract;
using FlexPage.Helpers;
using System.Threading.Tasks;
using Pluritech.Log.Abstract;
using System.IO;
using Flexpage.Abstract.DTO;
using Flexpage.Service.Abstract;
using Pluritech.Notifications.Abstract;
using Pluritech.Authentication.Abstract;
using Pluritech.UserProfile.Abstract;
using Language = Flexpage.Domain.Entities.Language;
using ViewModel = Flexpage.Models.ViewModel;

namespace Flexpage.Controllers
{
    [FlexpageAdmin]
    public class AdminController : BasicController
    {
        private const string DATE_FORMAT = @"dd\/MM\/yyyy";
        public static string ParentKey = "Parent";
        private readonly IFileContentProcessor _fileService;
        private readonly IEnumProcessor _enumProcessor;
        private readonly IObjectProcessor _objectProcessor;
        private readonly IUser _user;
        private readonly IBlockProvider _blockProvider;
        private readonly ILog _log;
        private readonly ISettingsProvider _websiteSettings;
        private int websiteID { get; set; }

        public AdminController(IFlexpageRepository repository, IFlexpageSettings settings, IFileContentProcessor fileService, IPermissionsService permService, IUser user, IBlockProvider blockProvider
            , IPropertyProvider propertyService, ILocalization localization, IContactProvider contactProvider, IEnumProcessor enumProcessor, IObjectProcessor objectProcessor, IFlexpage flexpageProcessor,
            ILog log, IUserGeneralPermissionProvider userGeneralPermissionProvider, ISettingsProvider websiteSettings) 
            : base(repository, settings, localization, permService, propertyService, contactProvider, flexpageProcessor, userGeneralPermissionProvider)
        {
            _fileService = fileService;            
            _enumProcessor = enumProcessor;
            _objectProcessor = objectProcessor;
            _log = log;
            _user = user;
            _blockProvider = blockProvider;
            _websiteSettings = websiteSettings;

            websiteID = _settings.WebsiteID;
        }
        public ActionResult Index()
        {
            return Redirect("/Account/login");
        }

        public ViewResult PagesManager()
        {
            return View("~/Areas/Flexpage/Views/Admin/PagesManager.cshtml");
        }

        [HttpGet]
        public ViewResult InstagramSuccessAuth()
        {
            return View();
        }

        public ActionResult GetPluriworksPopup()
        {
            return null;
        }
        #region Blocks        

        [HttpPost]
        public PartialViewResult CreateOrUpdateSocialMediaFeed(SocialMediaFeedModel model)
        {
            if(!string.IsNullOrEmpty(model.TwitterLogin))
            {
                model.TwitterLogin = model.TwitterLogin.TrimStart('@');
            }

            model.Apply(_repository);

            return GetEditorFor(model);
        }

        [HttpPost]
        public PartialViewResult UpdateAdvertisement(AdvertisementModel model, string command, string parameters)
        {
            string updateCommand = command.ToLower().Trim();

            if(updateCommand == "save")
            {
                model.Update();
                var advertisement = model.Apply(_repository) as Advertisement;
                _repository.ApplyChanges();
                model.Assign(advertisement, _repository.GetEntityList<AdvertisementImage>());
            }
            return GetEditorFor(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult UpdateAdvertisementImage(AdvertisementImageModel model, string command, string parameters)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    string c = command.ToLower().Trim();
                    if(c == "save")
                    {
                        model.Update();
                        var image = model.Apply(_repository) as AdvertisementImage;
                        _repository.ApplyChanges();
                        model.Assign(image, AdvertisementImageModel.GetAllLanguages(_repository, _settings));
                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return GetEditorFor(model);
        }

        // model сделать string
        [HttpPost]
        public PartialViewResult UpdateMultiColumn(MultiColumnModel model, string command, string parameters)
        {
            if(ModelState.IsValid)
            {
                string c = command.ToLower().Trim();
                ModelState.Clear();
                model.Update();

                if(c == "setpredefined")
                {
                    model.SetPredefined(int.Parse(parameters));
                }
                else
                if(c == "changelanguage")
                {
                    model.SelectLanguage(parameters as string);
                }
                else
                if(c == "addcolumn")
                {
                    model.AddColumn();
                }
                else
                if(c == "deletecolumn")
                {
                    int v = int.Parse(parameters);
                    model.DeleteColumn(v);
                    ModelState.Clear();
                }
                else
                    if(c == "save")
                {
                    model.Update();
                    model.Apply(_repository);
                    _repository.ApplyChanges();
                }

                return GetEditorFor(model);
            }
            return PartialView(ModelState.ToString());
        }

        // model сделать string
        [HttpPost]
        public PartialViewResult UpdateTabContainer(TabContainerModel model, string command, string parameters)
        {
            if(ModelState.IsValid)
            {
                string c = command.ToLower().Trim();
                ModelState.Clear();
                model.Update();

                if(c == "changelanguage")
                {
                    model.SelectLanguage(parameters as string);
                }
                else
                if(c == "orderup")
                {
                    int v = int.Parse(parameters);
                    model.ChangeOrder(v, -1);
                    ModelState.Clear();
                }
                else
                if(c == "orderdown")
                {
                    int v = int.Parse(parameters);
                    model.ChangeOrder(v, 1);
                    ModelState.Clear();
                }
                else
                if(c == "addtab")
                {
                    model.AddTab();
                }
                else
                if(c == "deletetab")
                {
                    int v = int.Parse(parameters);
                    model.DeleteTab(v);
                }
                else
                    if(c == "save")
                {
                    model.Apply(_repository);
                    _repository.ApplyChanges();
                }
                else if(c == "generatemenucode")
                {
                    model.PageUrl = parameters;
                    model.GenerateMenuCode();
                }
                else if (c == "changeEditorType")
                {
                    ModelState.Clear();
                }

                return GetEditorFor(model);
            }
            return PartialView(ModelState.ToString());
        }


        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdatePicture(PictureModel model, string command, string parameters)
        {
            ModelState.Clear();

            try
            {
                command = command ?? "";
                switch(command.ToLowerInvariant())
                {
                    case "changeeditortype":
                        {

                        }
                        break;
                    case "changepicturelanguage":
                        {
                            if(parameters is string)
                            {
                                model.Update();
                                model.SelectPictureLocalization(parameters);
                            }
                            break;
                        }
                    case "changedescrlanguage":
                        {
                            if(parameters is string)
                            {
                                model.Update();
                                model.SelectDescriptionLocalization(parameters);
                            }
                            break;
                        }
                    case "save":
                    default:
                        {
                            model.Update();
                            model.Apply(_repository);
                        }
                        break;
                }

                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }
        public ViewResult SitemapFilesMigration()
        {
            return View("~/Areas/Flexpage/Views/Flexpage/Editors/SiteMapManagerDBMigrate.cshtml", "");
        }
        [HttpPost]
        public PartialViewResult UpdateSitemap(SiteMapManager2Model model, string command, string parameters)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    string c = command.ToLower().Trim();
                    if(c == "save")
                    {
                        model.Update();
                        model.Apply(_repository);
                        _repository.ApplyChanges();

                        // load default stage to avoid exception at the last line GetEditorFor()
                        model.Load(_repository, null);
                    }
                    else if(c == "changesitemap")
                    {
                        model.LoadSitemap(_repository, parameters);
                    }
                    else if(c == "addsitemap")
                    {
                        model.AddSitemap(_repository, parameters);
                    }
                    else if(c == "addnewnode")
                    {
                        model.AddNewNode(_repository, _settings);
                    }
                    else if(c == "canceledit")
                    {
                        model.CancelEdit(_repository, parameters);
                    }
                    else if(c == "deletenode")
                    {
                        model.DeleteNode(_repository, parameters);
                    }
                    ModelState.Clear();
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return GetEditorFor(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateVideo(VideoModel model, string command, string parameters)
        {
            model.MediaType = new MediaType() { Name = MediaTypeName.Video.GetDisplay() };
            return UpdateMedia(model, command, parameters);
        }
        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateAudio(AudioModel model, string command, string parameters)
        {
            model.MediaType = new MediaType() { Name = MediaTypeName.Audio.GetDisplay() };
            return UpdateMedia(model, command, parameters);
        }
        [NonAction]
        [ValidateInput(false)]
        public PartialViewResult UpdateMedia(MediaModel model, string command, string parameters)
        {
            ModelState.Clear();
            // model.LoadLanguages(_repository);

            try
            {
                command = command ?? "";
                model.Update();
                switch(command.ToLowerInvariant())
                {
                    case "deletethumb":
                        {
                            model.DeleteThumb();
                            break;
                        }
                    case "deletevideo":
                        {
                            model.DeleteMedia();
                            break;
                        }
                    case "save":
                        {
                            model.Apply(_repository);
                            break;
                        }
                    case "videouploaded":
                        {
                            model.CurrentLocalization.MediaUrl = parameters;
                            break;
                        }
                    case "thumbuploaded":
                        {
                            model.CurrentLocalization.ThumbUrl = parameters;
                            break;
                        }
                    case "changevideolanguage":
                        {
                            model.SelectMediaLanguage(parameters);
                            break;
                        }
                    case "changeinfolanguage":
                        {
                            model.SelectInfoLanguage(parameters);
                            break;
                        }
                }
                model.Update();

                return GetEditorFor(model);
            }
            catch 
            {
                return GetEditorFor(model);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateAudioPlaylist(AudioPlaylistModel model, string command, string parameters)
        {
            model.MediaType = new MediaType() { Name = MediaTypeName.AudioPlaylist.GetDisplay() };
            model.Items.ForEach(i => i.MediaType = new MediaType() { Name = MediaTypeName.Audio.GetDisplay() });
            return UpdateMediaPlaylist(model, command, parameters);
        }
        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateVideoPlaylist(VideoPlaylistModel model, string command, string parameters)
        {
            model.MediaType = new MediaType() { Name = MediaTypeName.VideoPlaylist.GetDisplay() };
            model.Items.ForEach(i => i.MediaType = new MediaType() { Name = MediaTypeName.Video.GetDisplay() });
            return UpdateMediaPlaylist(model, command, parameters);
        }
        [NonAction]
        [ValidateInput(false)]
        public PartialViewResult UpdateMediaPlaylist(MediaPlaylistModel model, string command, string parameters)
        {

            ModelState.Clear();
            // model.LoadLanguages(_repository);

            // try
            // {
            command = command ?? "";
            model.Update();
            switch(command.ToLowerInvariant())
            {
                case "deletethumb":
                    {
                        model.CurrentItem.DeleteThumb();
                        break;
                    }
                case "deletevideo":
                    {
                        model.CurrentItem.DeleteMedia();
                        break;
                    }
                case "save":
                    {
                        model.Apply(_repository);
                        break;
                    }
                case "videouploaded":
                    {
                        model.CurrentItem.CurrentLocalization.MediaUrl = parameters;
                        break;
                    }
                case "thumbuploaded":
                    {
                        model.CurrentItem.CurrentLocalization.ThumbUrl = parameters;
                        break;
                    }
                case "changevideolanguage":
                    {
                        model.CurrentItem.SelectMediaLanguage(parameters);
                        break;
                    }
                case "changeinfolanguage":
                    {
                        model.SelectInfoLanguage(parameters);
                        break;
                    }
                case "changevideoinfolanguage":
                    {
                        model.CurrentItem.SelectInfoLanguage(parameters);
                        break;
                    }
                case "selectitem":
                    {
                        int v;
                        if(int.TryParse(parameters, out v))
                            model.SelectItem(v);
                        break;
                    }
                case "additem":
                    {

                        model.AddItem();
                        break;
                    }
                case "deleteitem":
                    {
                        int v;
                        if(int.TryParse(parameters, out v))
                            model.DeleteItem();
                        break;
                    }
            }
            model.Update();

            return GetEditorFor(model);
            //}
            //catch (Exception ex)
            //{
            //    return GetEditorFor(model);
            //}
        }

        /* 
        [HttpPost]
        public PartialViewResult UpdateMedia(VideoModel model, string command, string parameters)
        {
            ModelState.Clear();

            model.LoadLanguages(_repository);

            try
            {
                command = command ?? "";
                switch (command.ToLowerInvariant())
                {
                    case "save":
                    default:
                        {
                            model.Update();
                            model.Apply(_repository);
                        }
                        break;
                }

                return GetEditorFor(model);
            }
            catch (Exception ex)
            {
                return GetEditorFor(model);
            }
        } */

        [HttpPost]
        public PartialViewResult UpdateGallery(GalleryModel model, string command, string parameters)
        {
            try
            {
                command = command ?? "";
                switch(command.ToLowerInvariant())
                {
                    case "changeeditortype":
                        {
                            ModelState.Clear();
                        }
                        break;
                    case "changeimagelanguage":
                        {
                            ModelState.Clear();
                            if(parameters is string)
                            {
                                model.Update();
                                model.SelectImageDetailsLocalization(parameters);
                            }
                            break;
                        }
                    case "addimage":
                        ModelState.Clear();
                        if(parameters is string)
                        {
                            model.Update();
                            model.AddImage(parameters);
                        }
                        break;
                    case "deleteimage":
                        ModelState.Clear();
                        if(parameters is string)
                        {
                            model.Update();
                            model.DeleteImage(parameters);
                        }
                        break;
                    case "save":
                    default:
                        {
                            model.Update();
                            model.Apply(_repository);
                        }
                        break;
                }

                return GetEditorFor(model);
            }
            catch 
            {
                return GetEditorFor(model);
            }
        }
        [HttpPost]
        public PartialViewResult UpdateMaintenance(MaintenanceModel model, string command, string parameters)
        {
            try
            {
                command = command ?? "";
                switch(command.ToLowerInvariant())
                {
                    case "save":
                    default:
                        {
                            model.Update();
                            model.Apply(_repository);
                            ConfigurationManager.AppSettings["FP:Maintenance"] = "";
                        }
                        break;
                }

                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }


        [HttpPost]
        public PartialViewResult UpdateTinyUrl(TinyUrlModel model, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if(c == "save")
                {
                    model.Update();
                    model.Apply(_repository);
                    _repository.ApplyChanges();
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateMenu(MenuModel model, string command, string parameters)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                try
                {
                    string c = command.ToLower().Trim();
                    //model.Load(_repository);
                    if (c == "changesitemap" && parameters is string)
                    {
                        model.Update();
                    }
                    else if (c == "save")
                    {

                        model.Update();
                        model.Apply(_repository);
                        _repository.ApplyChanges();

                    }
                    return GetEditorFor(model);
                }
                catch
                {
                    return GetEditorFor(model);
                }
            }
            else
            {
                return GetInvalidModelResponse(ModelState);
            }

        }

        #endregion
        public ActionResult GetPermissionsForParentObject(int objectID)
        {
            var parrent = _permissionsService.GetParentObjectPermissions(objectID);
            if (parrent == null)
            {
                return null;
            }
            return Content(parrent.Permissions);
        }

        // private PartialViewResult GetBlockSettingsView(string block, string alias, string parameters)
        public override PartialViewResult GetBlockSettingsView(BlockCommandModel model)
        {
            // model.Url = Request.Url.AbsoluteUri;
            ViewModel m = null;
            if(string.Compare(model.Command, "addCss", true) == 0)
            {
                m = new BlockStyleModel(_settings, _flexpageProcessor);
                m.Load(_repository, model);
            }
            else
            {
                Abstract.DTO.IBlockModel blockModel = _flexpageProcessor.LoadBlockEditorModel(model.BlocklistID, model.ID, model.BlockType, model.BlockAlias,model.BlockAfter,
                    Request.QueryString);
                if (blockModel != null)
                {
                    return GetEditorForBlock(blockModel);
                }

                if(model.BlockType == Pluritech.FileSystem.Abstract.Enum.ModelType.FileHistory.ToString())
                {
                    Pluritech.FileSystem.Abstract.DTO.FileHistoryModel editorModel = _fileService.LoadFileHistoryModel(model.ID);
                    editorModel.Title = model.Title;
                    return PartialView(string.Format(EditorTemplatePath, model.BlockType), editorModel);
                }

                m = CreateAndLoadModel(model, true, model.Title);

                if(m is ObjectPropertiesModel)
                {
                    ((ObjectPropertiesModel)m).Properties = _propertyService.GetObjectCustomPropertiesModel(m.ID);
                    ((ObjectPropertiesModel)m).Permissions = _permissionsService.GetObjectPermissions(m.ID);
                    ((ObjectPropertiesModel)m).Publication = _contactProvider.GetFileShortcutPublications(m.ID);
                }
                if((model.BlockType == "Browser" || model.BlockType == "BrowserContacts") && model.ID <= 0 && string.IsNullOrEmpty(model.BlockAlias))
                {
                    m.Apply(_repository);
                }
            }
            return GetEditorFor(m);
        }

        /// <summary>
        /// Return View for edit block settings
        /// </summary>
        /// <param name="model">Popup content model</param>
        /// <returns></returns>
        public PartialViewResult GetPopupEditBlockContent(BlockCommandModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/PopupEditContent.cshtml", model);
        }
        
        [HttpPost]
        /// <summary>
        /// DexEx callback control requires that resulted view returns the same control's view
        /// </summary>
        public PartialViewResult GetPopupContent(BlockCommandModel model)
        {
            switch (model.Command)
            {
                case "choose":
                    return GetPopupChooseObjectContent(model);
                case "edit":
                case "addCss":
                    return GetPopupEditBlockContent(model);
                case "add":
                    // 'alias' - alias of the page
                    // 'block' - block name to be added
                    // if no 'block' - show available block list
                    // else - create new block and show its' settings
                    if(string.IsNullOrEmpty(model.BlockType))
                    {
                        return PartialView("~/Areas/Flexpage/Views/Admin/AddBlock.cshtml", _blockProvider.GetAvailableBlocks());
                    }
                    else
                    // Not used for now
                    if(model.BlockType == "MultiContent")
                    {
                        return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/MultiContent.cshtml");
                    }
                    else
                    {
                        model.Command = "edit";
                        model.BlockAlias = _repository.CreateNewAlias;
                        // model.Url = Request.RawUrl;
                        return GetPopupEditBlockContent(model);
                    }
                default:
                    return GetCustomPopupContent(model);
            }
        }

        /// <summary>
        /// Action for DevEx Html editor control for switching between modes
        /// </summary>
        /// <returns></returns>
        public PartialViewResult HtmlEditorPartialCommand(string Name)
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/HtmlEditorPartial.cshtml", new Flexpage.Abstract.DTO.HtmlEditorModel()
            {
                Name = Name,
                Width = new Unit(100, UnitType.Percentage),
                Height = new Unit(400, UnitType.Pixel),
                MaxFileSize = _settings.HtmlEditorMaxFileSize,
            });
        }

        /// <summary>
        /// Returns all available advertisement tags.
        /// </summary>
        /// <returns>A list of advertisement tags.</returns>
        public JsonResult GetAdvertisementTags()
        {
            IEnumerable<string> data = AdvertisementModel.GetAllowedTags(_repository);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Function to delete the news record
        /// </summary>
        /// <param name="ID">ID of the record</param>
        /// <returns></returns>
        public void DeleteEvent([ModelBinder(typeof(DevExpressEditorsBinder))] int ID)
        {
            //delete the PageBlock by alias
            //only int alias can be deleted
            try
            {
                _repository.DeleteEntity<Event>(ID);
                _repository.ApplyChanges();
            }
            catch { }
        }

        /// <summary>
        /// Action to delete the tinyUrl record
        /// </summary>
        /// <param name="ID">ID of the record</param>
        /// <returns></returns>
        public ActionResult DeleteTinyUrl([ModelBinder(typeof(DevExpressEditorsBinder))] int ID)
        {
            //delete the PageBlock by alias
            //only int alias can be deleted
            try
            {
                _repository.DeleteEntity<TinyUrl>(ID);
                _repository.ApplyChanges();
            }
            catch { }
            return TinyUrlGrid();
        }


        /// <summary>
        /// Action to delete an advertisement image
        /// </summary>
        /// <param name="ID">ID of a block</param>
        /// <returns></returns>
        public ActionResult DeleteAdvertisementImage([ModelBinder(typeof(DevExpressEditorsBinder))] int ID)
        {
            _repository.DeleteEntity<AdvertisementImage>(ID);
            _repository.ApplyChanges();

            return AdvertisementsGrid();
        }

        [ValidateInput(false)]
        public ActionResult HtmlEditorPartial_ProcessingImageUpload(string folderPath)
        {
            var fileSaveSettings = new HtmlEditorFileSaveSettings();
            fileSaveSettings.FileSystemSettings.UploadFolder = folderPath;
            HtmlEditorExtension.SaveUploadedFile(fileSaveSettings, null);
            return null;
        }

        [ValidateInput(false)]
        public ActionResult UploadControl_ProcessingImageUpload(string folderPath, string uploadControlName)
        {
            UploadControlExtension.GetUploadedFiles(uploadControlName, null, (s, e) =>
            {
                uploadControl_FileUploadComplete(folderPath, e);
            });
            return null;
        }

        [ValidateInput(false)]
        public ActionResult HtmlEditorPartial_DocumentUpload(string folderPath, string name)
        {
            var settings = new MVCxHtmlEditorDocumentSelectorSettings(null);
            settings.Enabled = true;
            settings.UploadSettings.Enabled = true;
            settings.CommonSettings.RootFolder = folderPath;
            HtmlEditorExtension.SaveUploadedDocument(name, settings);
            return null;
        }

        [ValidateInput(false)]
        public ActionResult HtmlEditorPartial_InsertPWLink()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/PwLinkPopup.cshtml");
        }

        private void uploadControl_FileUploadComplete(string folder, FileUploadCompleteEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(folder))
                folder = "/Content/Images/User/";
            if(e.UploadedFile.IsValid)
            {
                string resultFileName = folder + e.UploadedFile.FileName.Replace("&", string.Empty);
                string resultFilePath = HttpContext.Server.MapPath(resultFileName);
                int idx = 0;
                while(System.IO.File.Exists(resultFilePath))
                {
                    resultFileName = string.Format("{2}{0}_{1}", ++idx, e.UploadedFile.FileName, folder);
                    resultFilePath = HttpContext.Server.MapPath(resultFileName);
                }
                var f = HttpContext.Server.MapPath(folder);
                DirectoryInfo dirInfo = new DirectoryInfo(f);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                e.UploadedFile.SaveAs(resultFilePath, true);
                e.CallbackData = resultFileName;
            }
        }

        public ActionResult AdvertisementsGrid()
        {
            List<AdvertisementImage> banners = _repository.GetEntityList<AdvertisementImage>();
            Dictionary<int, Language> languages = AdvertisementImageModel.GetAllLanguages(_repository, _settings);
            var images = new List<AdvertisementImageModel>();
            banners.ForEach(b =>
            {
                var adv = new AdvertisementImageModel(_settings, _flexpageProcessor);
                adv.Assign(b, new object[] { languages });
                images.Add(adv);
            });

            return PartialView("~/Areas/Flexpage/Views/Admin/AdvertisementsGrid.cshtml", images);
        }

        public ActionResult TinyUrlGrid()
        {
            List<TinyUrlModel> records = new List<TinyUrlModel>();

            ViewData["Language"] = _repository.Languages.ToList();

            foreach(var rec in _repository.GetTinyUrls(websiteID))
            {
                TinyUrlModel model = new TinyUrlModel(_settings, _flexpageProcessor);
                model.Assign(rec);
                records.Add(model);
            }
            return PartialView("~/Areas/Flexpage/Views/Admin/TinyUrlGrid.cshtml", records);
        }


        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateBrowser(BrowserModel model, string command, string parameters)
        {
            model.Update();
            model.Apply(_repository);
            _repository.ApplyChanges();
            return GetEditorFor(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateBrowserContacts(BrowserContactsModel model, string command, string parameters)
        {
            model.Update();
            model.Apply(_repository);
            _repository.ApplyChanges();
            return GetEditorFor(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateFavoritesList(FavoritesListModel model, string command, string parameters)
        {
            model.Update();
            model.Apply(_repository);
            _repository.ApplyChanges();
            return GetEditorFor(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateAddPageToFavorites(AddPageToFavoritesModel model, string command, string parameters)
        {
            model.Update();
            model.Apply(_repository);
            _repository.ApplyChanges();
            return GetEditorFor(model);
        }

        [HttpPost]
        public PartialViewResult UpdateFolderContent(FolderContentModel model, string command, string parameters)
        {
            ModelState.Clear();
            try
            {
                string c = command.ToLower().Trim();
                switch(c)
                {
                    case "add_column":
                        model.AddColumn();
                        break;
                    case "remove_column":
                        model.RemoveColumn(parameters);
                        break;
                    case "save":
                        model.Update();
                        model.Apply(_repository);
                        _repository.ApplyChanges();
                        break;
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }
        [HttpPost]
        public PartialViewResult UpdateContactsEnumeration(ContactsEnumerationModel model, string command, string parameters)
        {
            ModelState.Clear();
            try
            {
                string c = command.ToLower().Trim();
                switch(c)
                {
                    case "add_column":
                        model.AddColumn();
                        break;
                    case "remove_column":
                        model.RemoveColumn(parameters);
                        break;
                    case "save":
                        model.Update();
                        model.Apply(_repository);
                        _repository.ApplyChanges();
                        break;
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        [HttpPost]
        public string ContactDetails_GeneratePassword()
        {
            return FlexPage2.Areas.Flexpage.Infrastructure.FormsAuthProvider.GeneratePassword();
        }

        [HttpPost]
        public PartialViewResult UpdateContactDetails(ContactDetailsModel model, string command, string parameters)
        {
            ModelState.Clear();
            
                try
            {
                string c = command.ToLower().Trim();
                switch(c)
                {
                    case "add_column":
                        model.AddColumn();
                        break;
                    case "remove_column":
                        model.RemoveColumn(_repository, parameters);
                        break;
                    case "save":
                        model.Update();
                        model.Apply(_repository);
                        _repository.ApplyChanges();
                        break;
                }
                ViewBag.IsAdmin = _repository.GetUser() > 1;
                return GetEditorFor(model);
            }
            catch
            {
                ViewBag.IsAdmin = _repository.GetUser() > 1;
                return GetEditorFor(model);
            }

        }
        public PartialViewResult UpdateFolderTreeList(FolderTreeListModel model, string command, string parameters)
        {
            ModelState.Clear();
            try
            {
                string c = command.ToLower().Trim();
                switch(c)
                {
                    case "save":
                        model.Update();
                        model.Apply(_repository);
                        _repository.ApplyChanges();
                        break;
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }
        public PartialViewResult GetPopupChooseObjectContent(BlockCommandModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/PopupChooseContent.cshtml", model);
        }

        public PartialViewResult GetObjectSelectorView(BlockCommandModel model)
        {
            ViewModel m = CreateAndLoadModel(model);
            return GetSelectorFor(m, model.Title);
        }

        private PartialViewResult GetSelectorFor(ViewModel target,string title)
        {
            string s = target.GetType().Name;
            if(s.EndsWith("Model"))
                s = s.Remove(s.Length - 5);
            target.FillViewData(ViewData, _repository, title);
            PartialViewResult pr = PartialView(string.Format("~/Areas/Flexpage/Views/Flexpage/Selectors/{0}.cshtml", s), target);
            return pr;
        }

        public ActionResult FolderSelector_BindingPartial()
        {
            ViewModel model = new FolderSelectorModel(_settings, _flexpageProcessor);
            model.Load(_repository, null);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Selectors/FolderSelectorGrid.cshtml", model);
        }

        #region Admin`s menu


        [HttpPost]
        public string ToggleTabVisibility(string tabID)
        {
            return _repository.ToggleTabVisibility(Convert.ToInt32(tabID));
        }

        /// <summary>
        /// Action to delete the block
        /// </summary>
        /// <param name="blockID">ID of the block</param>
        /// <returns></returns>
        public string DeleteBlock(int blockID, string blockName, int blocklistID = 0)
        {
            try
            {
                if (blockName == "Event")
                    DeleteEvent(blockID);
                else
                {
                    _repository.RemoveBlockFromBlockList(blockID, blocklistID);
                    _repository.ApplyChanges();
                }
            }
            catch { }
            return "Ok";
        }

        /// <summary>
        /// Action to save custom css style for the block
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public void SaveBlockStyle(BlockStyleModel model)
        {
            try
            {
                _repository.SaveBlockStyle(model.BlockID, model.CustomCssClass, model.CustomView,model.CustomCss);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public string AddToRss(int blockID)
        {
            _repository.AddToRss(blockID, "New rss item added");
            return String.Empty;
        }
        #endregion

        public ViewResult RSSManager()
        {
            RSSModel rssModel = new RSSModel(_repository);
            return View("~/Areas/Flexpage/Views/Admin/RSSManager.cshtml", rssModel);
        }

        public ActionResult DeleteRss([ModelBinder(typeof(DevExpressEditorsBinder))] int ID)
        {
            _repository.DeleteEntity<RssFeed>(ID);
            _repository.ApplyChanges();

            return PartialView("~/Areas/Flexpage/Views/Admin/RSSManagerGrid.cshtml", RSSModel.RssGrid(_repository));
        }

        public ActionResult RSSGrid(int? rowKey)
        {
            if(rowKey != null)
            {
                RssFeed feed = _repository.GetByID<RssFeed>((int)rowKey);
                if(feed != null)
                {
                    feed.Archived = !feed.Archived;
                    _repository.ApplyChanges();
                }
            }
            return PartialView("~/Areas/Flexpage/Views/Admin/RSSManagerGrid.cshtml", RSSModel.RssGrid(_repository));
        }

        #region Enums
        public PartialViewResult EnumsGrid()
        {
            List<EnumModel> records = new List<EnumModel>();
            foreach(var rec in _repository.GetEntityList<Flexpage.Domain.Entities.Enum>())
            {
                EnumModel model = new EnumModel(rec, _settings, _flexpageProcessor, _repository);
                records.Add(model);
            }
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/Enums/_EnumsGrid.cshtml", records);
        }

        /// <summary>
        /// Action to delete enum
        /// </summary>
        /// <param name="ID">ID of the record</param>
        /// <returns></returns>
        public PartialViewResult DeleteEnum([ModelBinder(typeof(DevExpressEditorsBinder))] int ID)
        {
            //delete the PageBlock by alias
            //only int alias can be deleted
            try
            {
                if (_propertyService.GetObjectPropertiesEnumCount(ID) == 0)
                {
                    _repository.DeleteEntity<Flexpage.Domain.Entities.Enum>(ID);
                    _repository.ApplyChanges();
                }
                else
                {
                    throw new Exception("This enum can't be deleted because it's referenced at least by one custom property");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            return EnumsGrid();
        }

        [HttpPost]
        public PartialViewResult UpdateEnums(EnumListModel model, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if(c == "save")
                {
                    model.Update();
                    model.Apply(_repository);
                    _repository.ApplyChanges();
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        public PartialViewResult EnumValuesGrid(int enumid, string langCode)
        {
            var source = _repository.GetByID<Flexpage.Domain.Entities.Enum>(enumid);
            List<EnumValueModel> records;
            if(source != null)
                records = source.EnumValue.Select(e => new EnumValueModel(e, _settings, _flexpageProcessor, _repository, langCode)).ToList();
            else
                records = new List<EnumValueModel>();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/Enums/_EnumValuesGrid.cshtml", records);
        }

        [HttpPost]
        public PartialViewResult UpdateEnum(EnumModel model)
        {
            try
            {
                model.Update();
                model.Apply(_repository);
                _repository.ApplyChanges();
                return EnumsGrid();
            }
            catch
            {
                return EnumsGrid();
            }
        }

        [HttpPost]
        public PartialViewResult AddEnum(EnumModel model)
        {
            try
            {
                model.Update();
                model.Apply(_repository);
                _repository.ApplyChanges();
                return EnumsGrid();
            }
            catch
            {
                return EnumsGrid();
            }
        }

        [HttpPost]
        public PartialViewResult UpdateEnumValuesList(EnumModel model, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if (c == "save")
                {
                    model.Update();
                    model.Apply(_repository);
                    _repository.ApplyChanges();
                }
                else
                if (c == "changelanguage")
                {
                    model.SetCurrentLanguage(parameters);
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        [HttpPost]
        public PartialViewResult UpdateEnumValue(EnumValueModel model, int enumid, string name, string langCode)
        {
            try
            {
                model.CurrentLanguage = langCode;
                model.Update();
                model.Apply(_repository);
                _repository.ApplyChanges();
                return EnumValuesGrid(enumid, langCode);
            }
            catch
            {
                return EnumValuesGrid(enumid, langCode);
            }
        }

        [HttpPost]
        public PartialViewResult AddEnumValue(EnumValueModel model, int enumid, string name, string langCode)
        {
            try
            {
                model.CurrentLanguage = langCode;
                model.Update();
                model.Apply(_repository);
                _repository.ApplyChanges();
                return EnumValuesGrid(enumid, langCode);
            }
            catch
            {
                return EnumValuesGrid(enumid, langCode);
            }
        }

        [HttpPost]
        public PartialViewResult DeleteEnumValue(int id, int enumid, string langCode)
        {
            try
            {
                _repository.DeleteEntity<EnumValue>(id);
                _repository.ApplyChanges();
                return EnumValuesGrid(enumid, langCode);
            }
            catch
            {
                return EnumValuesGrid(enumid, langCode);
            }
        }
        public ActionResult Enums()
        {
            EnumsModel model = new EnumsModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings) { }, "");
            return PartialView("~/Areas/Flexpage//Views/Shared/DisplayTemplates/Enums/_Enums.cshtml", model);
        }

        public ActionResult PWEnums()
        {
            //Pluritech.Pluriworks.Service.DTO.EnumsModel model = new Pluritech.Pluriworks.Service.DTO.EnumsModel();
            //model.Load(_repository, new BlockCommandModel(_settings) { }, "");
            //return PartialView("~/Areas/Flexpage//Views/Shared/DisplayTemplates/Enums/_PWEnums.cshtml", model);
            return PartialView("~/Areas/Flexpage//Views/Shared/DisplayTemplates/Enums/_PWEnums.cshtml", _enumProcessor.GetEnumsModel());
        }

        public PartialViewResult PWEnumsGrid()
        {
            //return null;
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/Enums/_PWEnumsGrid.cshtml", _enumProcessor.GetEnums());
        }

        public PartialViewResult PWDeleteEnum([ModelBinder(typeof(DevExpressEditorsBinder))] int ID)
        {
            try
            {
                if (_propertyService.GetObjectPropertiesEnumCount(ID) == 0)
                {
                    _enumProcessor.DeleteEnum(ID);
                    _enumProcessor.ApplyChanges();
                }
                else
                {
                    throw new Exception("This enum can't be deleted because it's referenced at least by one custom property");
                }
            }
            catch(Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            return PWEnumsGrid();
        }

        [HttpPost]
        public PartialViewResult PWUpdateEnums(Pluritech.Pluriworks.Service.DTO.EnumListModel model, string command, string parameters)
        {
            try
            {
                _enumProcessor.UpdateEnums(model);
                _enumProcessor.ApplyChanges();
                return PWEnumsGrid();
            }
            catch
            {
                return PWEnumsGrid();
            }
       }
        public PartialViewResult PWEnumValuesGrid(int enumid, string langCode)
        {
            langCode = String.IsNullOrEmpty(langCode) ? _localization.GetCurrentOrDefaultLangCode() : langCode;
            List<EnumValueModel> records= _enumProcessor.GetEnumValues(enumid, langCode).Select(e => new EnumValueModel(e, _settings, _flexpageProcessor, _repository, langCode)).ToList();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/Enums/_PWEnumValuesGrid.cshtml", records);
        }

        public ActionResult PWEnumWrapper(BlockCommandModel model)
        {
           
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/PWEnumWrapper.cshtml", model);
        }

        public ActionResult PWEnumEditor(int ID)
        {
            var model = _enumProcessor.GetEnumByID(ID);
            model.CurrentLanguage = String.IsNullOrEmpty(model.CurrentLanguage) ? _localization.GetCurrentOrDefaultLangCode() : model.CurrentLanguage;
            model.LanguageSelector.LangCodes = _enumProcessor.GetEnumModelLangCodes(model);
            if (model.LanguageSelector.LangCodes.Count == 0)
            {
                model.LanguageSelector.LangCodes.Add(model.CurrentLanguage);
            }
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/PWEnum.cshtml", model);
        }

        [HttpPost]
        public PartialViewResult PWUpdateEnum(Pluritech.Pluriworks.Service.DTO.EnumModel model)
        {
            try
            {
                _enumProcessor.AddEnum(model);
                _enumProcessor.ApplyChanges();
                return PWEnumsGrid();
            }
            catch
            {
                return PWEnumsGrid();
            }
        }

        [HttpPost]
        public PartialViewResult PWAddEnum(Pluritech.Pluriworks.Service.DTO.EnumModel model)
        {
            try
            {
                _enumProcessor.AddEnum(model);
                _enumProcessor.ApplyChanges();
                return PWEnumsGrid();
            }
            catch
            {
                return PWEnumsGrid();
            }
        }
        #endregion

        [HttpPost]
        public PartialViewResult PWUpdateEnumValuesList(Pluritech.Pluriworks.Service.DTO.EnumModel model, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if (c == "save")
                {
                    _enumProcessor.AddEnum(model);
                    _enumProcessor.ApplyChanges();
                }
                else
                if (c == "changelanguage")
                {
                    _enumProcessor.SetCurrentLanguage(model, parameters);
                }
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/PWEnum.cshtml", model);
            }
            catch
            {
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/PWEnum.cshtml", model);
            }
        }

        [HttpPost]
        public PartialViewResult PWUpdateEnumValue(EnumValueModel model, int enumid, string name, string langCode)
        {
            try
            {
                model.CurrentLanguage = String.IsNullOrEmpty(langCode) ? _localization.GetCurrentOrDefaultLangCode() : langCode;
               
                _enumProcessor.AddEnumValue(new Pluritech.Pluriworks.Service.DTO.EnumValueModel()
                {
                    ID = model.ID,
                    Value = model.Value,
                    CurrentLanguage = model.CurrentLanguage,
                    CurrentText = model.CurrentText,
                    Index = model.Index,
                    EnumID = enumid,
                    Text = model.Text
                });
                _enumProcessor.ApplyChanges();
                return PWEnumValuesGrid(enumid, langCode);
            }
            catch
            {
                return PWEnumValuesGrid(enumid, langCode);
            }
        }

        [HttpPost]
        public PartialViewResult PWAddEnumValue(EnumValueModel model, int enumid, string name, string langCode)
        {
            try
            {
                model.CurrentLanguage = String.IsNullOrEmpty(langCode) ? _localization.GetCurrentOrDefaultLangCode() : langCode;
                _enumProcessor.AddEnumValue(new Pluritech.Pluriworks.Service.DTO.EnumValueModel()
                {
                    ID = model.ID,
                    Value = model.Value,
                    CurrentLanguage = model.CurrentLanguage,
                    CurrentText = model.CurrentText,
                    Index = model.Index,
                    EnumID = enumid,
                    Text = model.Text
                });
                _enumProcessor.ApplyChanges();
                return PWEnumValuesGrid(enumid, langCode);
            }
            catch
            {
                return PWEnumValuesGrid(enumid, langCode);
            }
        }

        [HttpPost]
        public PartialViewResult PWDeleteEnumValue(int ID,int EnumID, string langCode)
        {
            try
            {
                _enumProcessor.DeleteEnumValue(ID);
                _enumProcessor.ApplyChanges();
                return PWEnumValuesGrid(EnumID, langCode);
            }
            catch
            {
                return PWEnumValuesGrid(EnumID, langCode);
            }
        }

        public ViewResult SubscriptionListManager()
        {
            SubscriptionListModel subModel = new SubscriptionListModel(_settings, _flexpageProcessor);
            return View("~/Areas/Flexpage/Views/Flexpage/Editors/SubscriptionList.cshtml", subModel);
        }


        public PartialViewResult SubscriptionsGrid(string langCode)
        {
            var source = _repository.GetEntityList<Flexpage.Domain.Entities.Subscription>();
            List<SubscriptionModel> records;
            if(source != null)
                records = source.Select(e => new SubscriptionModel(e, _settings, _flexpageProcessor, _repository, langCode)).ToList();
            else
                records = new List<SubscriptionModel>();
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/SubscriptionsGrid.cshtml", records);
        }

        [HttpPost]
        public PartialViewResult UpdateSubscriptionList(SubscriptionListModel model, string command, string parameters)
        {
            try
            {
                string c = command.ToLower().Trim();
                if(c == "changelanguage")
                {
                    model.SetCurrentLanguage(parameters);
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult UpdateDisclaimer(DisclaimerModel model, string command, string parameters)
        {
            if(command.ToLower().Trim() == "changelanguage")
            {
                model.SelectLocalization();
            }
            else
            {
                model.Update();
                model.Apply(_repository);
                _repository.ApplyChanges();
            }

            return GetEditorFor(model);
        }

        [HttpPost]
        public PartialViewResult UpdateSubscription(SubscriptionModel model, string command, string parameters)
        {
            try
            {
                ModelState.Clear();
                string c = command.ToLower().Trim();
                if(c == "save")
                {
                    model.Update(_repository);
                    model.Apply(_repository);
                    _repository.ApplyChanges();
                }
                else
                if(c == "changelanguage")
                {
                    model.Update(_repository);
                    model.SetCurrentLanguage(parameters);
                   
                }
                else
                if(c == "deletetext")
                {
                    model.Update(_repository);
                    int v;
                    if(int.TryParse(parameters, out v))
                        model.DeleteText(v);
                }
                else
                if(c == "addtext")
                {
                    model.Update(_repository);
                    model.AddText(_repository);
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }

        public PartialViewResult SubscribersGrid(int subscriptionID, string langCode)
        {
            var source = _repository.GetEntityList<Flexpage.Domain.Entities.Subscriber>(e => e.SubscriptionID == subscriptionID)
                .Where(s => s.Void == false).ToList();
            List<SubscriberModel> records;
            if(source != null)
                records = source.Select(e => new SubscriberModel(e, _settings, _flexpageProcessor, _repository, langCode)).ToList();
            else
                records = new List<SubscriberModel>();
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/SubscribersGrid.cshtml", records);
        }

        public PartialViewResult RecycleBinGrid()
        {
            var model = new RecycleBinModel(_fileService);

            model.GridModel.ProcessCustomBinding(
                model.GetDataObjectsCount,
                model.GetDataObjects
            );

            return PartialView("~/Areas/Flexpage/Views/Flexpage/RecycleGrid.cshtml", model);
        }

        private ActionResult RecycleBin_GridCoreAction(GridViewModel gridViewModel)
        {
            var model = new RecycleBinModel(_fileService);

            model.GridModel = gridViewModel;
            model.GridModel.ProcessCustomBinding(
                model.GetDataObjectsCount,
                model.GetDataObjects
            );
            return PartialView("~/Areas/Flexpage/Views/Flexpage/RecycleGrid.cshtml", model);
        }

        public ActionResult RecycleBin_PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("RecycleBinGrid");
            viewModel.ApplyPagingState(pager);

            return RecycleBin_GridCoreAction(viewModel);
        }

        public ActionResult RecycleBin_SortingAction(GridViewColumnState column)
        {
            var viewModel = GridViewExtension.GetViewModel("RecycleBinGrid");
            viewModel.SortBy(column, true);

            return RecycleBin_GridCoreAction(viewModel);
        }

        public ActionResult RecycleBin_FilteringAction(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("RecycleBinGrid");
            viewModel.ApplyFilteringState(filteringState);

            return RecycleBin_GridCoreAction(viewModel);
        }

        public ActionResult RecycleBin_RestoreAction(int[] ids)
        {
            _fileService.FileObjectRestore(ids.ToList());
            return null;
        }
        public ActionResult RecycleBin_DeleteAction(int[] ids)
        {
            _fileService.DeleteCompletelyObjects(ids.ToList());
            return null;
        }
        

        public ActionResult GetListPermissions(int id)
        {
            var data = new List<ListPermissionsModel>();
            var models = _permissionsService.GetPermissionGeneral();

            models.ForEach(item =>
            {
                data.Add(new ListPermissionsModel() { Id = item.Id, Permission = item.Name });
            });

            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/GeneralPermissions/_GeneralPermissionsListGrid.cshtml", data);
        }

        protected string GetTypeName(string type) => type == "PersonObject" ? "Person" : "Security Group";

        public ActionResult GetWhoPermissions(int id, string newData)
        {
            var data = new List<WhoPermissionsModel>();
            var models = _permissionsService.GetGeneralPermissionObjects(id);
            var newDataObjects = JsonConvert.DeserializeObject<List<NewDataObject>>(newData).Where(w => w.PermissionId == id).ToList();

            models.ForEach(item =>
            {
                data.Add(new WhoPermissionsModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = GetTypeName(item.Type),
                    Permission = !item.IsDeny ? "Permit" : "Deny",
                    ListPermissionsId = id
                });
            });

            newDataObjects.ForEach(item =>
            {
                if ((item.Parameters.Equals("AddPerson") && !item.Parameters.Equals("Delete")) || (!string.IsNullOrEmpty(item.LastAction) && item.LastAction.Equals("AddPerson")))
                {
                   
                    var objectEntity = _repository.ObjectEntities().Include(i => i.ObjectType).Include(i => i.PersonShortcut).FirstOrDefault(p => p.PersonShortcut.ID == item.Id);

                    if (objectEntity != null &&  models.FirstOrDefault(w => w.Id == objectEntity.ID) == null)
                    {
                        data.Add(new WhoPermissionsModel()
                        {
                            Id = objectEntity.PersonShortcut.ID,
                            Name = objectEntity.PersonShortcut.Person.Name1 + ", " + objectEntity.PersonShortcut.Person.Name2,
                            Type = "Person",
                            Permission = !item.IsDeny ? "Permit" : "Deny",
                            ListPermissionsId = id
                        });
                    }
                }
                else if ((item.Parameters.Equals("AddGroup") && !item.Parameters.Equals("Delete")) || (!string.IsNullOrEmpty(item.LastAction) && item.LastAction.Equals("AddGroup")))
                {
                    
                    var objectEntity = _repository.ObjectEntities().Include(i => i.ObjectType).Include(i => i.Folder).FirstOrDefault(p => p.Folder.Name.Equals(item.Name));
                    if (objectEntity != null &&  models.FirstOrDefault(w => w.Id == objectEntity.ID) == null)
                    {
                        data.Add(new WhoPermissionsModel()
                        {
                            Id = objectEntity.Folder.ID,
                            Name = objectEntity.Folder.Name,
                            Type = "Security Group",
                            Permission = !item.IsDeny ? "Permit" : "Deny",
                            ListPermissionsId = id
                        });
                    }
                }
                else if ((item.Parameters.Equals("AddASPNETUser") && !item.Parameters.Equals("Delete")) || (!string.IsNullOrEmpty(item.LastAction) && item.LastAction.Equals("AddASPNETUser")))
                {
                    var objectEntity = _repository.ObjectEntities().Include(i => i.ObjectType).Include(i => i.PersonShortcut).FirstOrDefault(p => p.PersonShortcut.Person.Name2.Equals("ASPNET"));
                    if (objectEntity != null && models.FirstOrDefault(w => w.Id == objectEntity.ID) == null)
                    {
                        data.Add(new WhoPermissionsModel()
                        {
                            Id = objectEntity.PersonShortcut.ID,
                            Name = objectEntity.PersonShortcut.Person.Name1 + ", " + objectEntity.PersonShortcut.Person.Name2,
                            Type = "Person",
                            Permission = !item.IsDeny ? "Permit" : "Deny",
                            ListPermissionsId = id
                        });
                    }
                }
                else if ((item.Parameters.Equals("AddEveryone") && !item.Parameters.Equals("Delete")) || (!string.IsNullOrEmpty(item.LastAction) && item.LastAction.Equals("AddEveryone")))
                {
                    if (models.FirstOrDefault(w => w.Id == 0) == null)
                    {
                        data.Add(new WhoPermissionsModel()
                        {
                            Id = 0,
                            Name = "Everyone",
                            Type = "Security Group",
                            Permission = !item.IsDeny ? "Permit" : "Deny",
                            ListPermissionsId = id
                        });
                    }
                }

                if (item.Parameters.Equals("Delete"))
                {
                    var removeData = data.FirstOrDefault(w => w.ListPermissionsId == id && w.Name.Trim() == item.Name.Trim());
                    data.Remove(removeData);
                }

                if (item.Parameters.Equals("Permit"))
                {
                    var findData = data.FirstOrDefault(w => w.ListPermissionsId == id && w.Name.Trim() == item.Name.Trim());

                    if (findData != null)
                        findData.Permission = "Permit";
                    
                }
                else if (item.Parameters.Equals("Deny"))
                {
                    var findData = data.FirstOrDefault(w => w.ListPermissionsId == id && w.Name.Trim() == item.Name.Trim());

                    if (findData != null)
                        findData.Permission = "Deny";
                }
            });

            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/GeneralPermissions/_GeneralPermissionsWhoGrid.cshtml", data);
        }

        public ActionResult UpdateGeneralPermissions(GeneralPermissionsModel model)
        {
            if (!IsPWAdmin())
                throw new Exception("Not enough permissions");
            var jsonDates = JsonConvert.DeserializeObject<List<NewDataObject>>(model.JsonDataSave).ToList();

            foreach (var item in jsonDates)
            {
                if (item.Type.Equals(WhoPermissionsModel.NameTypePerson) || int.TryParse(item.Type, out int n))
                {
                    if (item.Id > 0)
                    {
                        var objectEntity = _repository
                            .ObjectEntities()
                            .Include(i => i.PersonShortcut)
                            .FirstOrDefault(p => p.PersonShortcut.ID == item.Id);

                        if (objectEntity != null)
                            if(item.IsRemoved)
                                _permissionsService.RemoveGeneralPermissionObjects(objectEntity.ID, item.PermissionId);
                            else
                                _permissionsService.SaveGeneralPermissionObject(objectEntity.ID, item.PermissionId,
                                item.IsDeny);
                    }
                    else
                    {
                        var objectEntity = _repository
                            .ObjectEntities()
                            .Include(i => i.PersonShortcut)
                            .FirstOrDefault(p => p.PersonShortcut.Person.Name2.Equals(WhoPermissionsModel.NameTypeASPNET));


                        if (objectEntity != null)
                            if (item.IsRemoved)
                                _permissionsService.RemoveGeneralPermissionObjects(objectEntity.ID, item.PermissionId);
                            else
                                _permissionsService.SaveGeneralPermissionObject(objectEntity.ID, item.PermissionId,
                                    item.IsDeny);
                    }

                }
                else if (item.Type.Equals(WhoPermissionsModel.NameTypeEveryone) || item.Name.Equals(WhoPermissionsModel.NameTypeEveryone))
                {
                    if (item.IsRemoved)
                        _permissionsService.RemoveGeneralPermissionObjects(0, item.PermissionId);
                    else
                        _permissionsService.SaveGeneralPermissionObject(0, item.PermissionId, item.IsDeny);
                }
                else if (item.Type.Equals(WhoPermissionsModel.NameTypeFolder))
                {
                    var objectEntity = _repository.ObjectEntities()
                        .Include(i => i.Folder)
                        .FirstOrDefault(p => p.Folder.Name.Equals(item.Name) || p.Folder.ID == item.Id);

                    if (objectEntity != null)
                        if (item.IsRemoved)
                            _permissionsService.RemoveGeneralPermissionObjects(objectEntity.ID, item.PermissionId);
                        else
                            _permissionsService.SaveGeneralPermissionObject(objectEntity.ID, item.PermissionId, item.IsDeny);
                }
            }

            return null;
        }

        [HttpPost]
        public JsonResult ContactsAdvancedSearch_Export(string path)
        {
            var query = "";

            try
            {
                var folder = _repository.GetFolder(path);
                var xml = Request.Params.Get("xml");
                if (!string.IsNullOrEmpty(xml))
                {
                    HttpCookie cookie = Request.Cookies[xml];
                    query = cookie.Value;
                    query = HttpUtility.UrlDecode(query);
                    var result = _contactProvider.SearchContacts(query);
                    result.ForEach(contactID =>
                    {
                        _contactProvider.AddPersonToFolder(contactID, folder == null ? 0 : folder.ID);
                        //var sc = _repository.AddObjectFolderPerson(contactID, folder == null ? 0 : folder.ID, _repository.GetUser());
                        //_repository.ApplyChanges();
                    });
                }
                else
                {
                    throw new Exception("xml is null");
                }
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Export\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true}");
        }
        [HttpPost]
        public JsonResult ContactsAdvancedSearch_CheckFolderDL(string path)
        {
            var result = false;
            try
            {
                var folder = _repository.GetFolder(path);
                var properties = _propertyService.GetObjectCustomProperties(folder.ID);
                result = properties.FirstOrDefault(p => p.Name == "DistributionList") != null;
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Check folder\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true,\"result\" : " + result.ToString().ToLower() + "}");
        }
        [HttpPost]
        public JsonResult ContactsAdvancedSearch_ConvertFoldertoDL(string path)
        {
            var result = false;
            try
            {
                var folder = _repository.GetFolder(path);

                var DL = _propertyService.EnumerateAllProperties().FirstOrDefault(p => p.Name == "DistributionList");

                _propertyService.SaveObjectCustomPropertiesValues(new Pluritech.Properties.Abstract.DTO.CustomPropertyModel()
                {
                    ObjectID = folder.ID,
                    Properties = new List<Pluritech.Properties.Abstract.DTO.CustomPropertyInfo>() {
                        new Pluritech.Properties.Abstract.DTO.CustomPropertyInfo() { ID= DL.ID, Name=DL.Name} }
                });
                result = true;
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Convert folder\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true,\"result\" : " + result.ToString().ToLower() + "}");
        }


        public ActionResult DeleteSubscriptions(SubscriptionModel model)
        {
            _repository.RemoveSubscription(model.ID);
            return SubscriptionsGrid(_settings.GetCurrentOrDefaultLangCode());
        }

        public ActionResult SubscriberForm(int subscriberId)
        {
            var model = new SubscriberModel(_settings, _flexpageProcessor);
            model.Load(_repository, new BlockCommandModel(_settings, _flexpageProcessor) { ID = subscriberId });

            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/SubscriberForm.cshtml", model);
        }

        public ActionResult UpdateSubscriber(SubscriberModel model)
        {
            model.Apply(_repository);
            return null;
        }

        public ActionResult DeleteSubscriber(int subscriberId)
        {
            _repository.RemoveSubscriber(subscriberId);
            return null;
        }

        public async Task<ActionResult> NotifySubscriber(int subscriberId)
        {
            var subscriber = _repository.GetByID<Subscriber>(subscriberId);
            if (!string.IsNullOrEmpty(subscriber.Email))
            {
                var mailHelper = new MailHelper();
                bool resultNotifing = await mailHelper.SendMail(ConfigurationManager.AppSettings["FromAddress"], subscriber.Email, "automatic message", "Notified");

                if (resultNotifing)
                {
                    subscriber.IsNotified = true;
                    _repository.ApplyChanges();
                }
            }
            return null;
        }

        public ActionResult PendingEvents()
        {
            ViewBag.IsAdmin = _repository.GetUser() > 1;
            PendingEventsModel model = new PendingEventsModel(_settings, _flexpageProcessor);
            model.Load(_repository, "", _contactProvider.GetPublishingQueueFiles(), _contactProvider.GetPublishingQueueItems(), _contactProvider.GetEventQueueStarted(), _contactProvider.GetEventQueueDelay(), true);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/PendingEvents.cshtml", model);
        }

        public ActionResult PendingEvents_FileUpdate()
        {
            var model = _contactProvider.GetPublishingQueueFiles();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/PendingEvents/_PendingFiles.cshtml", model);
        }

        public ActionResult PendingEvents_NotificationUpdate()
        {
            var model = _contactProvider.GetPublishingQueueItems();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/PendingEvents/_PendingNotifications.cshtml", model);
        }
        public ActionResult PendingEvents_Start()
        {
            PendingEventsModel model = new PendingEventsModel(_settings, _flexpageProcessor);
            _contactProvider.SetEventQueueStarted(true);
            model.Load(_repository, "", _contactProvider.GetPublishingQueueFiles(), _contactProvider.GetPublishingQueueItems(), _contactProvider.GetEventQueueStarted(), _contactProvider.GetEventQueueDelay(), true);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/PendingEvents/_PendingSettings.cshtml", model);
        }
        public ActionResult PendingEvents_Stop()
        {
            PendingEventsModel model = new PendingEventsModel(_settings, _flexpageProcessor);
            _contactProvider.SetEventQueueStarted(false);
            model.Load(_repository, "", _contactProvider.GetPublishingQueueFiles(), _contactProvider.GetPublishingQueueItems(), _contactProvider.GetEventQueueStarted(), _contactProvider.GetEventQueueDelay(), true);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/PendingEvents/_PendingSettings.cshtml", model);
        }
        public ActionResult PendingEvents_SetDelay(int delay)
        {
            PendingEventsModel model = new PendingEventsModel(_settings, _flexpageProcessor);
            _contactProvider.SetEventQueueDelay(delay);
            model.Load(_repository, "", _contactProvider.GetPublishingQueueFiles(), _contactProvider.GetPublishingQueueItems(), _contactProvider.GetEventQueueStarted(), _contactProvider.GetEventQueueDelay(), true);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/PendingEvents/_PendingSettings.cshtml", model);
        }
        public JsonResult PendingEvents_FileDelete(int ID)
        {
            try
            {
                _contactProvider.DeleteFromPublishingQueue(new List<int>() { ID }, false);
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Pending Events. Delete File\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true}");
        }
        public JsonResult PendingEvents_NotificationDelete(int ID, int ContactID, string Address)
        {
            try
            {
                var delete = _contactProvider.GetPublishingQueueItems().Where(q => q.FileID == ID && q.ContactID == ContactID && q.Address == Address).ToList();
                _contactProvider.DeletePublishingQueueItem(delete);
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Pending Events. Delete Notification\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true}");
        }

        public ActionResult Logs(string alias)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Logs/Logs.cshtml");
        }

        public ActionResult GetLogs()
        {
            var models = _log.GetAllLog();
            
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Logs/LogsGrid.cshtml", models);
        }
        /// <summary>
        /// View with Notifications Popup
        /// </summary>
        /// <returns></returns>
        public PartialViewResult NotificationsPersonPopup(BlockCommandModel blockCommandModel)
        {
            NotificationsModel model = new NotificationsModel(_settings, _flexpageProcessor);
            dynamic stuff = JsonConvert.DeserializeObject(blockCommandModel.Parameters);
            int folderId = Convert.ToInt32( stuff.folderID.Value);
            int contactId = Convert.ToInt32(stuff.ContactID.Value);
            int contactShortcutID = Convert.ToInt32(stuff.ContactShortcutID.Value);
            int notification = _contactProvider.GetPublishingContactOverride(folderId, contactShortcutID) ?? _contactProvider.GetPublishingContactDefault(folderId);
            
            model.Load(_repository, _contactProvider, blockCommandModel, contactId, contactShortcutID, eContactType.Person, folderId, (Notification)notification, "Notifications");
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactsEnumeration/_Notifications.cshtml", model);
        }
        /// <summary>
        /// View with Notifications Popup
        /// </summary>
        /// <returns></returns>
        public PartialViewResult NotificationsCompanyPopup(BlockCommandModel blockCommandModel)
        {
            NotificationsModel model = new NotificationsModel(_settings, _flexpageProcessor);
            dynamic stuff = JsonConvert.DeserializeObject(blockCommandModel.Parameters);
            int folderId = Convert.ToInt32(stuff.folderID.Value);
            int contactId = Convert.ToInt32(stuff.ContactID.Value);
            int contactShortcutID = Convert.ToInt32(stuff.ContactShortcutID.Value);
            int notification = _contactProvider.GetPublishingContactOverride(folderId, contactShortcutID) ?? _contactProvider.GetPublishingContactDefault(folderId);
            model.Load(_repository, _contactProvider, blockCommandModel, contactId, contactShortcutID, eContactType.Company, folderId, (Notification)notification, "Notifications");
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactsEnumeration/_Notifications.cshtml", model);
        }
        /// <summary>
        /// View with Email Overriding Popup
        /// </summary>
        /// <returns></returns>
        public PartialViewResult EmailOverridingPersonPopup(BlockCommandModel blockCommandModel)
        {
            EmailOverridingModel model = new EmailOverridingModel(_settings, _flexpageProcessor);
            dynamic stuff = JsonConvert.DeserializeObject(blockCommandModel.Parameters);
            int folderId = Convert.ToInt32(stuff.folderID.Value);
            int contactId = Convert.ToInt32(stuff.ContactID.Value);
            int contactShortcutID = Convert.ToInt32(stuff.ContactShortcutID.Value);
            model.Load(_repository, _contactProvider, blockCommandModel, contactId, contactShortcutID, eContactType.Person, folderId, "Email Overriding");
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactsEnumeration/_EmailOverriding.cshtml", model);
        }
        /// <summary>
        /// View with Email Overriding Popup
        /// </summary>
        /// <returns></returns>
        public PartialViewResult EmailOverridingCompanyPopup(BlockCommandModel blockCommandModel)
        {
            EmailOverridingModel model = new EmailOverridingModel(_settings, _flexpageProcessor);
            dynamic stuff = JsonConvert.DeserializeObject(blockCommandModel.Parameters);
            int folderId = Convert.ToInt32(stuff.folderID.Value);
            int contactId = Convert.ToInt32(stuff.ContactID.Value);
            int contactShortcutID = Convert.ToInt32(stuff.ContactShortcutID.Value);
            model.Load(_repository, _contactProvider, blockCommandModel, contactId, contactShortcutID, eContactType.Company, folderId, "Email Overriding");
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactsEnumeration/_EmailOverriding.cshtml", model);
        }
        /// <summary>
        /// View with Email Overriding Popup
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveNotifications(NotificationsModel model)
        {
            try
            {
                int? telecomID = null;
                _contactProvider.SavePublishingContactOverride(model.ApplyToAllContactFolders?0: model.FolderID, model.СontactShortcutID, (int?)model.Notification);
                if(model.Type== TypeEmailOverriding.UseDefault)
                {
                    telecomID = null;
                    model.Email = String.Empty;
                } else if(model.Type == TypeEmailOverriding.Overide)
                {
                    telecomID = model.OverideID;
                    model.Email = String.Empty;
                }
                else
                {
                    telecomID = 0;
                }
                _contactProvider.SaveEmailAddressOverride(model.ApplyToAllContactFolders ? 0 : model.FolderID, model.СontactShortcutID,model.ContactType, telecomID, model.Email);
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Save Publishing Contact Notifications\",\"message\" : \"" + e.Message.Split('.').First() + "\"}}");
            }

            return Json("{\"success\": true}");
        }
        /// <summary>
        /// View with Email Overriding Popup
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveEmailOverriding(EmailOverridingModel model)
        {
            try
            {
                int? telecomID = null;
                if (model.Type == TypeEmailOverriding.UseDefault)
                {
                    telecomID = null;
                    model.Email = String.Empty;
                }
                else if (model.Type == TypeEmailOverriding.Overide)
                {
                    telecomID = model.OverideID;
                    model.Email = String.Empty;
                }
                else
                {
                    telecomID = 0;
                }
                _contactProvider.SaveEmailAddressOverride(model.ApplyToAllContactFolders ? 0 : model.FolderID, model.СontactShortcutID, model.ContactType, telecomID, model.Email);
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Save Publishing Contact Email Overriding\",\"message\" : \"" + e.Message.Split('.').First() + "\"}}");
            }

            return Json("{\"success\": true}");
        }
        /// <summary>
        /// View with Email Overriding Popup
        /// </summary>
        /// <returns></returns>
        public JsonResult DeletePublishingContactOverrides(int folderId)
        {
            try
            {
                _contactProvider.DeletePublishingContactOverrides(new List<int>() { folderId });
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Delete Publishing Contact Overrides\",\"message\" : \"" + e.Message.Split('.').First() + "\"}}");
            }

            return Json("{\"success\": true}");
        }
        /// <summary>
        /// View with Email Overriding Popup
        /// </summary>
        /// <returns></returns>
        public JsonResult ObjectTimeToLeaveReset(int folderId, bool recursive)
        {
            try
            {
                if (!_contactProvider.ObjectTimeToLeaveReset(new List<int>() { folderId }, null, recursive, _repository.GetUser()))
                {
                    throw new Exception(
                    "Some objects can't be accessed recursive because you have no permission to change them.");
                }
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Time to leave has been reset.\",\"message\" : \"" + e.Message.Split('.').First() + "\"}}");
            }

            return Json("{\"success\": true}");
        }
        public ActionResult Publication_Update(int ID)
        {
           var model= _contactProvider.GetFileShortcutPublications(ID);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/_BrowserCustomPublicationGrid.cshtml", model);
        }
        public ActionResult Publication_Edit(int ID, PublicationModel model)
        {
            _contactProvider.UpdateFileShortcutPublications(ID, new List<PublicationModel>() { model });
          var modelView = _contactProvider.GetFileShortcutPublications(ID);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/_BrowserCustomPublicationGrid.cshtml", modelView);
        }

        [ValidateInput(false)]
        public ActionResult Page_UploadCustomCss(string folderPath, string uploadControlName)
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

        public ActionResult WebsiteSettings()
        {
            return View("~/Areas/Flexpage/Views/Admin/WebsiteSettings.cshtml", _websiteSettings.GetSettingsForPage());
        }

        public ActionResult UpdateWebsiteSettings(List<WebsiteSettingsModel> model)
        {
            _websiteSettings.SaveSettings(model);
            return new HttpStatusCodeResult(200);
        }
    }
}