using DevExpress.Utils.Extensions;
using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Code.Helpers;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Flexpage.Domain.Search;
using Flexpage.Models;
using Flexpage.Service.Abstract;
using FlexPage2.Areas.Flexpage.Helpers;
using FlexPage2.Areas.Flexpage.Models.Enums;
using Newtonsoft.Json;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.FileSystem.Abstract;
using Pluritech.FileSystem.Abstract.DTO;
using Pluritech.Permissions.Abstract;
using Pluritech.Permissions.Abstract.DTO;
using Pluritech.Properties.Abstract;
using Pluritech.Properties.Abstract.DTO;
using Pluritech.Settings.Abstract;
using Pluritech.Shared.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using IEnumProcessor = Pluritech.Pluriworks.Service.Abstract.IEnumProcessor;
using IObjectProcessor = Pluritech.Pluriworks.Service.Abstract.IObjectProcessor;
using IObjectQueryBuilder = Pluritech.Shared.Abstract.IObjectQueryBuilder;
using Flexpage.Infrastructure;
using FlexPage2.Areas.Flexpage.Models.CustomPropertyAddModel;
using Pluritech.Authentication.Abstract;
using Pluritech.UserProfile.Abstract;
using ViewModel = Flexpage.Models.ViewModel;

namespace Flexpage.Controllers
{
    public delegate ActionResult PartialViewDelegate();

    public class FlexpageController : BasicController
    {
        private readonly IFileContentProcessor _fileService;
        private readonly IFlexpageService _flexpageService;
        private readonly IFileConverter _fileConverter;
        private readonly IEnumProcessor _enumProcessor;
        private readonly IObjectProcessor _objectProcessor;
        private readonly IUserSettingsProvider _userSettings;
        private readonly IPageProvider _pageProvider;
        private readonly IUser _user;
        private readonly ISettingsProvider _websiteSettings;

        private int websiteID { get; set; }

        public FlexpageController(
            IFlexpageRepository repository,
            IFlexpageSettings settings,
            IFileContentProcessor fileService,
            IFlexpageService flexpageService,
            ILocalization localization,
            IFileConverter fileConverter,
            IPropertyProvider propertyService,
            IPermissionsService permService,
            [Ninject.Named("FoldersQueryBuilder")]IObjectQueryBuilder _foldersQueryBuilder,
            [Ninject.Named("ContactsQueryBuilder")]IObjectQueryBuilder _contactsQueryBuilder,
            IContactProvider contactProvider,
            IEnumProcessor enumProcessor,
            IObjectProcessor objectProcessor,
            IFlexpage flexpageProcessor,
            IUserSettingsProvider userSettings,
            IPageProvider pageProvider,
            IUser user,
            ISettingsProvider websiteSettings,
            IUserGeneralPermissionProvider userGeneralPermissionProvider
            )
            : base(repository, settings, localization, permService, propertyService, contactProvider, flexpageProcessor, userGeneralPermissionProvider)
        {
            _fileService = fileService;
            _flexpageService = flexpageService;
            _fileConverter = fileConverter;
            _enumProcessor = enumProcessor;
            _objectProcessor = objectProcessor;
            //TODO remove this temp code
            _repository.QueryFolders = _foldersQueryBuilder;
            _repository.QueryContacts = _contactsQueryBuilder;
            _userSettings = userSettings;
            _pageProvider = pageProvider;
            _user = user;
            _websiteSettings = websiteSettings;

            websiteID = settings.WebsiteID;
        }

        public PartialViewResult WrapBlockCreation(string alias, PartialViewDelegate viewCreationRoutine)
        {
            try
            {
                return viewCreationRoutine() as PartialViewResult;
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

        public string Index()
        {
            return "This is default Flexpage landing page";
        }

        public EmptyResult RenewSession()
        {
            return new EmptyResult();
        }

        public Boolean CheckAuth()
        {
            return Request.RequestContext.HttpContext.User.Identity.IsAuthenticated;
        }

        public ActionResult NotFound()
        {
            string alias = Request.RawUrl.TrimStart('/');
            return CmsPage(alias);
        }

        [RoleAccess]
        [HttpGet]
        public ActionResult CmsPage(string alias)
        {
            //check customErrors redirect
            string aspxerrorpath = Request.Params["aspxerrorpath"];
            if (!String.IsNullOrEmpty(aspxerrorpath))
            {
                alias = aspxerrorpath.TrimStart('/');
            }
            alias = alias.TrimEnd('/');
            ViewBag.QueryString = HttpUtility.UrlDecode(Request.QueryString.ToString());
            //RouteData.Values[]
            //check TinyUrl first
            ActionResult tiny = TinyUrl(alias);
            if (tiny != null)
            {
                return tiny;
            }

            //check if the page exists
            if (_pageProvider.IsPageExists(alias))
            {
                return View("~/Areas/Flexpage/Views/Shared/CmsPage.cshtml", (object)alias);
                // return View("~/Areas/Flexpage/Views/Shared/CmsPage.cshtml", new BlockModel(_settings));
            }
            else
            {
                try { Response.StatusCode = 404; } // try/catch required for unit testing
                catch { }
                return View(string.Format("~/Areas/Flexpage/Views/{0}/CmsPageEmpty.cshtml", IsAdmin() ? "Admin" : "Shared"), (object)alias);
            }
        }

        public ActionResult TinyUrl(string alias)
        {
            var tinyUrl = _repository.GetTinyUrl(alias, websiteID);
            if (tinyUrl != null)
            {
                Dictionary<string, object> queryParams = new Dictionary<string, object>();
                if (RouteData.Values.ContainsKey("Request.QueryString"))
                {
                    NameValueCollection qs = RouteData.Values["Request.QueryString"] as NameValueCollection;
                    if (qs != null)
                    {
                        qs.CopyTo(queryParams);
                    }
                }

                if (tinyUrl.Language != null && !string.IsNullOrWhiteSpace(tinyUrl.Language.Code))
                {
                    if (queryParams.ContainsKey("lang"))
                    {
                        queryParams["lang"] = tinyUrl.Language.Code;
                    }
                    else
                    {
                        queryParams.Add("lang", tinyUrl.Language.Code);
                    }
                }

                string query = "";
                if (queryParams.Count > 0)
                {
                    query = "?" + string.Join("&", queryParams.Select(q => q.Key + "=" + q.Value.ToString()).ToArray());
                }

                var url = tinyUrl.NavigateUrl + query;

                if (!tinyUrl.IsShowDestinationUrl && Request?.Url != null)
                {

                    HttpContext.Server.TransferRequest(tinyUrl.NavigateUrl);
                    return null;
                }

                return Redirect(url);
            }
            return null;
        }

        public PartialViewResult Popup()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/PopupDialog.cshtml");
        }
        public PartialViewResult Maintenance()
        {
            MaintenanceModel m = ViewModel.Create("MaintenanceModel", _settings, _flexpageProcessor) as MaintenanceModel;
            m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = "0", ID = Models.BlockModel.NewStaticBlockID });

            return PartialView("~/Areas/Flexpage/Views/Flexpage/Maintenance.cshtml", m);
        }

        #region Blocks
        public PartialViewResult MultiColumn(string alias)
        {
            return WrapBlockCreation(alias, () =>
            {
                ViewModel m = ViewModel.Create("MultiColumnModel", _settings, _flexpageProcessor);
                m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = alias, ID = Models.BlockModel.NewStaticBlockID });
                m.IsStatic = true;
                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
            });
        }

        public PartialViewResult WebForm(string alias)
        {
            return WrapBlockCreation(alias, () =>
            {
                ViewModel m = ViewModel.Create("WebFormModel", _settings, _flexpageProcessor);
                m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = alias, ID = Models.BlockModel.NewStaticBlockID });
                m.IsStatic = true;
                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
            });
            // return PartialView("~/Areas/PluriCloud/Views/Cloud/Echo.cshtml", String.Format("<b>FlexpageController.WebForm (alias = {0}): {1}</b><br>{2}<br>{3}<br>", alias, ex.Message, ex.Source, ex.StackTrace)); 
        }
        public PartialViewResult Menu(string alias)
        {
            return WrapBlockCreation(alias, () =>
            {
                var staticBlocks = Session["StaticBlocks"] as HashSet<eBlockType>;
                if (staticBlocks == null)
                {
                    staticBlocks = new HashSet<eBlockType>();
                }
                staticBlocks.Add(eBlockType.Menu);
                Session["StaticBlocks"] = staticBlocks;
                ViewModel m = ViewModel.Create("MenuModel", _settings, _flexpageProcessor);
                m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = alias, ID = Models.BlockModel.NewStaticBlockID });
                m.IsStatic = true;
                //if (string.IsNullOrEmpty(alias))
                //    return PartialView("~/Areas/Flexpage/Views/Flexpage/Menu.cshtml", m);
                //else
                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
            });
        }

        public PartialViewResult Video(string alias)
        {
            return WrapBlockCreation(alias, () =>
            {
                ViewModel m = ViewModel.Create("VideoModel", _settings, _flexpageProcessor);
                m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = alias, ID = Models.BlockModel.NewStaticBlockID });
                m.IsStatic = true;
                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
            });
        }

        public ActionResult RSSFeed()
        {
            try
            {
                RSSModel m = new RSSModel(_repository);

                return this.Content(Helpers.ViewHelper.RenderPartialViewToString(this, "~/Areas/Flexpage/Views/Flexpage/RSSFeed.cshtml", m.Feeds), "application/rss+xml");
            }
            catch
            {
                return this.Content(View("~/Areas/Flexpage/Views/Flexpage/RSSFeed.cshtml").ToString(), "application/rss+xml");
            }
        }

        public ActionResult Logs()
        {
            ViewModel m = ViewModel.Create("LogsModel", _settings, _flexpageProcessor);
            m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = "0", ID = Models.BlockModel.NewStaticBlockID });

            return View("~/Areas/Flexpage/Views/Flexpage/Logs.cshtml", m);
        }

        public PartialViewResult Gallery(string alias)
        {
            return WrapBlockCreation(alias, () => {
                ViewModel m = ViewModel.Create("GalleryModel", _settings, _flexpageProcessor);
                m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = alias, ID = Models.BlockModel.NewStaticBlockID });
                m.IsStatic = true;

                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
            });
        }

        public PartialViewResult Advertisement(string alias)
        {
            return WrapBlockCreation(alias, () => {
                ViewModel m = ViewModel.Create("AdvertisementModel", _settings, _flexpageProcessor);
                m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = alias, ID = Models.BlockModel.NewStaticBlockID });
                m.IsStatic = true;
                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
            });
        }

        public PartialViewResult SocialMediaFeed(string alias, SocialMediaType mediaType)
        {
            return WrapBlockCreation(alias, () =>
            {
                var viewModel = ViewModel.Create("SocialMediaFeedModel", _settings, _flexpageProcessor) as SocialMediaFeedModel;

                viewModel.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = alias, ID = Models.BlockModel.NewStaticBlockID });
                viewModel.IsStatic = true;

                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", viewModel);
            });
        }

        #endregion

        public ActionResult ShowAdminControls(bool ShowAdminControls)
        {
            try
            {
                _settings.ShowAdminControls(ShowAdminControls);
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Error set ShowAdminControls\",\"message\" : \"" + e.Message.Split('.').First() + "\"}}");
            }

            return Json("{\"success\": true}");
        }

        public ActionResult GetPopup(int blocklistID, int number, int blockAfter, string pageUrl, int allowSave, string OneButtonText = "SAVE", string parameters = "")
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/PopupDialog.cshtml", new BlockCommandModel(_settings)
            {
                BlocklistID = blocklistID,
                IDPostfix = number.ToString(),
                BlockAfter = blockAfter,
                Url = pageUrl,
                AllowSave = allowSave,
                OneButtonText = string.IsNullOrEmpty(OneButtonText) ? "SAVE" : OneButtonText,
                Parameters = parameters,
            });
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

        private PartialViewResult GetSelectorFor(ViewModel target, string title)
        {
            string s = target.GetType().Name;
            if (s.EndsWith("Model"))
                s = s.Remove(s.Length - 5);
            target.FillViewData(ViewData, _repository, title);
            PartialViewResult pr = PartialView(string.Format("~/Areas/Flexpage/Views/Flexpage/Selectors/{0}.cshtml", s), target);
            return pr;
        }

        private ViewModel GenerateTreeListModel(int ID, bool PWBrowser, string saveSelectedKey, TypeContextMenu typeContextMenu = TypeContextMenu.Block)
        {
            FolderTreeListModel model = new FolderTreeListModel(_settings, _flexpageProcessor);
            model.PWBrowser = PWBrowser;
            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID });
            model.TypeContextMenu = typeContextMenu;
            model.SaveSelectedKey = saveSelectedKey;
            return model;
        }

        private ActionResult TreeList_PartialView(ViewModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FolderTreeList/_FolderTreeListView.cshtml", model);
        }

        public ActionResult FolderTreeList_BindingPartial(int blockID, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string saveSelectedKey = "")
        {
            return TreeList_PartialView(GenerateTreeListModel(blockID, PWBrowser, saveSelectedKey, typeContextMenu));
        }

        [HttpPost]
        public ActionResult FolderTreeList_BindingPartial_AfterUpdate(int blockID, FolderInfo folder, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string saveSelectedKey = "")
        {
            FolderTreeListModel model = GenerateTreeListModel(blockID, PWBrowser, saveSelectedKey, typeContextMenu) as FolderTreeListModel;
            //rename new folder
            _fileService.RenameFolder(folder.ID, folder.Name);
            return TreeList_PartialView(model);
        }

        [HttpPost]
        public ActionResult FolderTreeList_BindingPartial_AfterAdd(int blockID, FolderInfo folder, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string saveSelectedKey = "")
        {
            FolderTreeListModel model = GenerateTreeListModel(blockID, PWBrowser, saveSelectedKey, typeContextMenu) as FolderTreeListModel;
            //create new folder
            _fileService.CreateFolder(folder.Name, folder.ParentID);
            return TreeList_PartialView(model);
        }

        [HttpPost]
        public ActionResult FolderTreeList_BindingPartial_AfterDelete(int blockID, int ID, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string saveSelectedKey = "")
        {
            FolderTreeListModel model = GenerateTreeListModel(blockID, PWBrowser, saveSelectedKey, typeContextMenu) as FolderTreeListModel;

            //if selected folder was deleted, it cannot be opened on reload
            var sessionFolder = (string)Session["nodeSelected_fp_FolderTreeList_List" + model.ID];
            if (!String.IsNullOrEmpty(sessionFolder))
            {
                dynamic savedFolder = JsonConvert.DeserializeObject(sessionFolder);
                var selectedRow = (int)savedFolder["rowId"];
                if (selectedRow == ID)
                {
                    Session.Remove("nodeSelected_fp_FolderTreeList_List" + model.ID);
                }
            }

            //delete folder
            _fileService.DeleteFolder(ID);
            return TreeList_PartialView(model);
        }

        [HttpPost]
        public ActionResult FolderTreeList_CustomAction(string action, string args, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string saveSelectedKey = "")
        {
            switch (action)
            {
                case "refresh":
                    FolderTreeListModel model = GenerateTreeListModel(Int32.Parse(args), PWBrowser, saveSelectedKey, typeContextMenu) as FolderTreeListModel;
                    return TreeList_PartialView(model);
            }

            return new EmptyResult();
        }
        [HttpPost]
        public JsonResult FolderTreeList_ClickNode()
        {
            try
            {
                var saveSelectedKey = Request.Params.Get("saveSelectedKey");
                var args = Request.Params.Get("args");
                if (!String.IsNullOrEmpty(saveSelectedKey))
                {
                    Session[saveSelectedKey] = args;
                }
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Save Node Selected\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true}");

        }
        [HttpPost]
        public JsonResult FolderTreeList_UploadFile()
        {
            var selectFolderName = Request.Params.Get("selectFolderName");
            var filesExist = new List<string>();
            bool overwrite = false;
            try
            {
                if (!int.TryParse(Request.Params.Get("id"), out var blockID))
                {
                    throw new ArgumentNullException();
                }
                if (!int.TryParse(Request.Params.Get("folderId"), out var folderId))
                {
                    throw new ArgumentNullException();
                }
                // old school root folder
                if (folderId == 0)
                {
                    throw new Exception("Can't create a file in the root folder.");
                }
                if (!string.IsNullOrEmpty(Request.Params.Get("overwrite")))
                {
                    overwrite = Request.Params.Get("overwrite").Trim().ToLower() == "true";
                }

                foreach (string file in Request.Files)
                {
                    var upload = Request.Files[file];
                    var strDT = Request.Params.Get(file + "_dateModification").ToString();
                    DateTime dateModification = DateTime.Parse(strDT);
                    if (upload != null)
                    {
                        var namedStream = new NamedStream()
                        {
                            Stream = upload.InputStream,
                            Name = upload.FileName,
                            Overwrite = overwrite,
                            DateModification = dateModification
                        };
                        // save file
                        if (_fileService.UploadFile(folderId, namedStream, string.Empty) == false)
                        {
                            filesExist.Add(upload.FileName);
                        }
                    }
                }
                if (filesExist.Count() > 0 && overwrite == false)
                {
                    throw new Exception(String.Join(", ", filesExist.ToArray()) + " already exists. Do you want to replace it?");
                }
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Upload File\",\"message\" : \"" + e.Message + "\",\"overwrite\" : " + (filesExist.Count > 0).ToString().ToLower() + "}}");
            }

            return Json("{\"success\": true}");
        }

        [HttpPost]
        public JsonResult Browser_Paste(string IDsToCopy, string selectFolderName, string type, string action)
        {
            try
            {
                //selectFolderName = selectFolderName.TrimEnd(' ');
                if (type == "files")
                {
                    switch (action)
                    {
                        case "DeepCopy":
                            foreach (var fileID in IDsToCopy.Replace("files:", "").Split(','))
                            {
                                _fileService.CopyFile(int.Parse(fileID), selectFolderName, 0, "", true);
                            }
                            break;
                        case "Cut":
                            foreach (var fileID in IDsToCopy.Replace("files:", "").Split(','))
                            {
                                _fileService.MoveFile(int.Parse(fileID), selectFolderName);
                            }
                            break;
                        case "CopyShortcut":
                            foreach (var fileID in IDsToCopy.Replace("files:", "").Split(','))
                            {
                                _fileService.CopyFile(int.Parse(fileID), selectFolderName, 0, "", false);
                            }
                            break;
                        default:
                            throw new Exception("You need to copy a file or a folder first.");
                    }
                }
                else if (type == "folder")
                    switch (action)
                    {
                        case "DeepCopy":
                            _fileService.CopyFolder(int.Parse(IDsToCopy), selectFolderName);
                            break;
                        case "Cut":
                            _fileService.MoveFolder(int.Parse(IDsToCopy), selectFolderName);
                            break;
                        default:
                            throw new Exception("You need to copy a file or a folder first.");
                    }
                else
                    throw new Exception("You need to copy a file or a folder first.");
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Paste File\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true}");
        }


        public ActionResult FolderContentGrid(int ID, string selectFolderName = null, bool PWBrowser = false, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string filterCustomProperties = "", string filterExtension = "")
        {
            FolderContentModel model = FolderContentModel(ID, null, selectFolderName, true, PWBrowser, typeContextMenu, filterCustomProperties, filterExtension);
            if (model.ShowTiles)
            {
                return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FolderContent/_FolderContentTiles.cshtml", model);
            }
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FolderContent/_FolderContentGrid.cshtml", model);
        }

        public ActionResult FolderContent_Update(int ID, string selectFolderName = null, bool PWBrowser = false, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string filterCustomProperties = "", string filterExtension = "")
        {
            return FolderContent(ID, selectFolderName, null, false, PWBrowser, typeContextMenu, filterCustomProperties, filterExtension);
        }

        public ActionResult FolderContent(int ID, string selectFolderName = null, FolderContentModel model = null, bool ProcessCustomBinding = false, bool PWBrowser = false, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string filterCustomProperties = "", string filterExtension = "")
        {
            model = FolderContentModel(ID, model, selectFolderName, ProcessCustomBinding, PWBrowser, typeContextMenu, filterCustomProperties, filterExtension);
            ViewBag.IsAuthenticated = CheckAuth();
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FolderContent/_FolderContent.cshtml", model);
        }

        [HttpPost]
        public JsonResult FolderContent_RenameApply(int ID, int fileId, string newName, int blockId, string selectFolderName = null, bool PWBrowser = false, TypeContextMenu typeContextMenu = TypeContextMenu.Block)
        {
            try
            {
                FolderContentModel model = new FolderContentModel(_settings, _flexpageProcessor,
                    Request.UrlReferrer != null ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query) : null);
                _fileService.RenameFile(ID, newName.TrimEnd('.'));
                model.PWBrowser = PWBrowser;
                model.TypeContextMenu = typeContextMenu;
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Rename File\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true}");
        }
        [HttpPost]
        public ActionResult FolderContent_Delete(int ID, int fileId, string selectFolderName = null, bool PWBrowser = false, TypeContextMenu typeContextMenu = TypeContextMenu.Block)
        {
            try
            {
                FolderContentModel model = new FolderContentModel(_settings, _flexpageProcessor, Request.UrlReferrer != null ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query) : null);
                model.PWBrowser = PWBrowser;
                model.TypeContextMenu = typeContextMenu;
                _fileService.DeleteFile(fileId);

                return new HttpStatusCodeResult(200);
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, e.Message);
            }
        }

        public FolderContentModel FolderContentModel(int ID, FolderContentModel model = null, string selectFolderName = null, bool ProcessCustomBinding = true
            , bool PWBrowser = false, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string filterCustomProperties = "", string filterExtension = "")
        {
            var viewModel = GridViewExtension.GetViewModel("fp_FolderContent_Grid" + ID.ToString());

            model = FolderContent_PrepareModel(ID, model, viewModel, selectFolderName, PWBrowser, typeContextMenu, filterCustomProperties, filterExtension, ProcessCustomBinding);
            return model;
        }

        private FolderContentModel FolderContent_PrepareModel(int ID, FolderContentModel model, GridViewModel gridViewModel
            , string selectFolderName, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block
            , string filterCustomProperties = "", string filterExtension = "", bool ProcessCustomBinding = true)
        {
            if (model == null)
            {
                model = new FolderContentModel(_settings, _flexpageProcessor, Request.UrlReferrer != null ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query) : null);
            }
            model.PWBrowser = PWBrowser;
            model.TypeContextMenu = typeContextMenu;
            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, false, selectFolderName);
            if (!string.IsNullOrEmpty(filterCustomProperties))
            {
                ViewData["FilterCustomProperties"] = filterCustomProperties;
                model.FilterCustomProperties = filterCustomProperties;
            }
            if (!string.IsNullOrEmpty(filterExtension))
            {
                ViewData["FilterExtension"] = filterExtension;
                model.FilterExtension = filterExtension;
            }
            if (gridViewModel != null)
                model.GridModel = gridViewModel;
            if (ProcessCustomBinding)
            {
                if (model.ShowTiles)
                {
                    model.CardModel.ProcessCustomBinding(
                        model.GetDataObjectsCount,
                        model.GetDataObjects
                    );
                }
                else
                {
                    model.GridModel.ProcessCustomBinding(
                        model.GetDataObjectsCount,
                        model.GetDataObjects
                    );
                }
            }
            return model;
        }

        private ActionResult FolderContent_GridCoreAction(int ID, FolderContentModel model, GridViewModel gridViewModel
            , string selectFolderName, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block
            , string filterCustomProperties = "", string filterExtension = "", bool ProcessCustomBinding = true)
        {
            model = FolderContent_PrepareModel(ID, model, gridViewModel, selectFolderName, PWBrowser, typeContextMenu, filterCustomProperties, filterExtension, ProcessCustomBinding);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FolderContent/_FolderContentGrid.cshtml", model);
        }

        private ActionResult FolderContent_CardCoreAction(int ID, CardViewModel cardViewModel, string selectFolderName, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block)
        {
            FolderContentModel model = new FolderContentModel(_settings, _flexpageProcessor, Request.UrlReferrer != null ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query) : null);
            model.PWBrowser = PWBrowser;
            model.TypeContextMenu = typeContextMenu;
            model.Load(_repository, new BlockCommandModel(_settings) { ID = ID }, false, selectFolderName);
            model.CardModel = cardViewModel;
            model.CardModel.ProcessCustomBinding(
                model.GetDataObjectsCount,
                model.GetDataObjects
            );
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FolderContent/_FolderContentTiles.cshtml", model);
        }

        public ActionResult FolderContent_SortingAction(DevExpress.Web.Mvc.GridViewColumnState column, int ID, string selectFolderName, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string filterCustomProperties = "", string filterExtension = "")
        {
            var viewModel = GridViewExtension.GetViewModel("fp_FolderContent_Grid" + ID.ToString());
            viewModel.SortBy(column, true);

            return FolderContent_GridCoreAction(ID, null, viewModel, selectFolderName, PWBrowser, typeContextMenu, filterCustomProperties, filterExtension);
        }

        public ActionResult FolderContent_CardPagingAction(CardViewPagerState pager, int ID, string selectFolderName, bool PWBrowser)
        {
            var viewModel = CardViewExtension.GetViewModel("fp_FolderContent_Grid" + ID.ToString());
            viewModel.ApplyPagingState(pager);

            return FolderContent_CardCoreAction(ID, viewModel, selectFolderName, PWBrowser);
        }

        public ActionResult FolderContent_PagingAction(GridViewPagerState pager, int ID, string selectFolderName, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string filterCustomProperties = "", string filterExtension = "")
        {
            var viewModel = GridViewExtension.GetViewModel("fp_FolderContent_Grid" + ID.ToString());
            viewModel.ApplyPagingState(pager);

            return FolderContent_GridCoreAction(ID, null, viewModel, selectFolderName, PWBrowser, typeContextMenu, filterCustomProperties, filterExtension);
        }

        public ActionResult FolderContent_FilteringAction(GridViewFilteringState filteringState, int ID, string selectFolderName, bool PWBrowser, TypeContextMenu typeContextMenu = TypeContextMenu.Block, string filterCustomProperties = "", string filterExtension = "")
        {
            var viewModel = GridViewExtension.GetViewModel("fp_FolderContent_Grid" + ID.ToString());
            viewModel.ApplyFilteringState(filteringState);

            return FolderContent_GridCoreAction(ID, null, viewModel, selectFolderName, PWBrowser, typeContextMenu, filterCustomProperties, filterExtension);
        }

        public ActionResult FolderContent_GetDetailsRow(ObjectInfo model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/FolderContentDetails.cshtml", model);
        }

        public ActionResult DownloadFile(int id, int revisionID = 0)
        {
            try
            {
                NamedStream stream = _fileService.DownloadFile(id, revisionID);
                string contentType = MimeMapping.GetMimeMapping(stream.Name);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = stream.Name,
                    Inline = false
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(stream.Stream, contentType);
            }
            catch (Exception ex)
            {
                throw new HttpException(404, "File is corrupted", ex);
            }
        }

        public ActionResult Browser(string alias, bool basic = false)
        {
            return WrapBlockCreation(alias, () => {
                BrowserModel model = new BrowserModel(_settings, _flexpageProcessor, basic);
                model.Load(_repository, alias, true);
                ViewBag.IsAdmin = IsAdmin();
                ViewBag.IsAuthenticated = CheckAuth();
                model.IsStatic = true;

                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", model);
            });
        }

        public ActionResult CustomProperties()
        {
            CustomPropertiesModel model = new CustomPropertiesModel(_settings, _flexpageProcessor);
            model.Load(_propertyService, _objectProcessor.GetObjectTypes(), _enumProcessor.GetEnums(), true);
            ViewBag.Edit = IsAdmin() || _permissionsService.UserHasGeneralPermission(eGeneralPermission.ManageCustomPropertiesList);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/CustomProperties.cshtml", model);
        }
        public ActionResult CustomPropertiesGrid(bool isNew = false, bool isEdit = false)
        {
            string[] EditFields = Request.Params["DXMVCGridEditFields"]?.Split(',') ?? new string[0];
            isNew = isNew || Request.Params["__DXCallbackArgument"].Contains("ADDNEWROW") || EditFields.Count() == 3;
            isEdit = isEdit || Request.Params["__DXCallbackArgument"].Contains("STARTEDIT") || EditFields.Count() == 2;
            CustomPropertiesModel model = new CustomPropertiesModel(_settings, _flexpageProcessor, isNew);
            model.Load(_propertyService, _objectProcessor.GetObjectTypes(), _enumProcessor.GetEnums(), true);
            ViewBag.isNew = isNew;
            ViewBag.isEdit = isEdit;
            ViewBag.Edit = IsAdmin() || _permissionsService.UserHasGeneralPermission(eGeneralPermission.ManageCustomPropertiesList);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/CustomProperties/_CustomPropertiesGrid.cshtml", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomProperties_Add(PropertyView viewModel)
        {
            var isNew = true;
            if (!IsAdmin() && !_permissionsService.UserHasGeneralPermission(eGeneralPermission.ManageCustomPropertiesList))
            {
                throw new Exception("You are not authorized to add a property.");
            };
            try
            {
                viewModel.ID = 0;
                viewModel.ObjectTypes = null;
                if (((int)viewModel.Type == 8 || (int)viewModel.Type == 11) && viewModel.EnumID == null)
                {
                    throw new Exception("Selecting an Enumeration is required for this type");
                }
                else if (!string.IsNullOrEmpty(viewModel.Name) && viewModel.Type != 0)
                {
                    if (_propertyService.EnumerateAllProperties().Where(p => p.Name == viewModel.Name.Replace(" ", "")).Count() > 0)
                    {
                        throw new Exception("Name must be unique");
                    }
                    _propertyService.SaveCustomProperty(viewModel);
                    isNew = false;
                }
                else
                {
                    throw new Exception("Fill in the required fields: Name, Type");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.Edit = true;
            ViewBag.isNew = isNew;
            ViewBag.isEdit = false;
            CustomPropertiesModel model = new CustomPropertiesModel(_settings, _flexpageProcessor, isNew);
            model.Load(_propertyService, _objectProcessor.GetObjectTypes(), _enumProcessor.GetEnums(), true);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/CustomProperties/_CustomPropertiesGrid.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CustomProperties_Edit(PropertyView viewModel)
        {
            var isEdit = true;
            if (!IsAdmin() && !_permissionsService.UserHasGeneralPermission(eGeneralPermission.ManageCustomPropertiesList))
            {
                throw new Exception("You are not authorized to edit a property.");
            };
            try
            {
                if (!string.IsNullOrEmpty(viewModel.Name))
                {
                    _propertyService.SaveCustomProperty(viewModel);
                    isEdit = false;
                }
                else
                {
                    throw new Exception("Fill in the required field: Name");
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.Edit = true;
            ViewBag.isNew = false;
            ViewBag.isEdit = isEdit;
            CustomPropertiesModel model = new CustomPropertiesModel(_settings, _flexpageProcessor);
            model.Load(_propertyService, _objectProcessor.GetObjectTypes(), _enumProcessor.GetEnums(), true);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/CustomProperties/_CustomPropertiesGrid.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CustomProperties_Delete(int ID)
        {
            if (!IsAdmin() && !_permissionsService.UserHasGeneralPermission(eGeneralPermission.ManageCustomPropertiesList))
            {
                throw new Exception("You are not authorized to delete a property.");
            };
            ViewBag.Edit = true;
            try
            {
                _propertyService.DeleteCustomProperty(ID);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            ViewBag.isNew = false;
            ViewBag.isEdit = false;
            CustomPropertiesModel model = new CustomPropertiesModel(_settings, _flexpageProcessor);
            model.Load(_propertyService, _objectProcessor.GetObjectTypes(), _enumProcessor.GetEnums(), true);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/CustomProperties/_CustomPropertiesGrid.cshtml", model);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CustomProperties_SaveObjectTypes(int ID, Pluritech.Properties.Abstract.ObjectType[] ObjectTypes)
        {
            ViewBag.Edit = true;
            try
            {
                var oldObjectTypes = _propertyService.GetObjectPropertiesTypes(ID).ToList();
                var newObjectTypes = ObjectTypes == null ? new List<Pluritech.Properties.Abstract.ObjectType>() : ObjectTypes.ToList();
                newObjectTypes.ForEach(ot =>
                {
                    if (!oldObjectTypes.Contains(ot))
                    {
                        _propertyService.SetObjectPropertiesTypes(ID, ot);
                    }
                });
                oldObjectTypes.ForEach(ot =>
                {
                    if (!newObjectTypes.Contains(ot))
                    {
                        _propertyService.DeleteObjectPropertiesTypes(ID, ot);
                    }
                });
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Error saving Custom Properties Object Types\",\"message\" : \"" + Regex.Replace(e.Message, @"\r\n?|\n", "") + "\"}}");
            }
            return Json("{\"success\": true, \"title\" : \"Save Custom Properties Object Types\",\"message\" : \"Object Types saved successfully\"}");
        }

        public ActionResult LinkContact(int id)
        {
            return Redirect(ConfigurationManager.AppSettings["FP:ContactsPageURL"].ToString().Replace("{id}", id.ToString()));
        }

        //[HttpPost]
        //public string AddScrollableNews(string blockID, string numberToUpload, string numberUploaded)
        //{
        //    string result = String.Empty;
        //    News block = _repository.GetByBlockID<News>(Convert.ToInt32(blockID));

        //    if (block != null)
        //    {
        //        IEnumerable<Event> events = _repository.GetEntityList<Event>().Where(r => r.IsVisible);
        //        List<EventModel> eventModels = new List<EventModel>();
        //        // TODO - move to Categories
        //        //if (block.TagName != null && !string.IsNullOrWhiteSpace(block.TagName))
        //        //{
        //        //    List<string> tags = block.TagName.Split(',').Select(t => t.Trim()).ToList();
        //        //    events = events.Where(r => tags.Contains(r.TagName));
        //        //}

        //        if (block.SortTypeAsc)
        //            events = events.OrderBy(r => r.StartDate);
        //        else
        //            events = events.OrderByDescending(r => r.StartDate);

        //        int toUpload = Convert.ToInt32(numberToUpload);
        //        int elementPos = Convert.ToInt32(numberUploaded);
        //        bool addFinishBlock = false;

        //        int availableNews = events.Count();
        //        if (availableNews < (elementPos + toUpload))
        //        {
        //            toUpload = toUpload - ((elementPos + toUpload) - availableNews);
        //            addFinishBlock = true;
        //        }

        //        for (int idx = 0; idx < toUpload; idx++)
        //        {
        //            Event rec = events.ElementAtOrDefault(elementPos);
        //            if (rec != null)
        //            {
        //                EventModel recModel = new EventModel(_settings, _flexpageProcessor);
        //                recModel.Assign(rec);
        //                recModel.IsHeaderLink = block.HeaderAsLink;
        //                recModel.FullNewsPageUrl = block.FullNewsPageUrl;
        //                recModel.LoadCmsText(_repository, false, "en");
        //                eventModels.Add(recModel);
        //                elementPos++;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }

        //        foreach (EventModel record in eventModels)
        //        {
        //            result += Flexpage.Helpers.ViewHelper.RenderPartialViewToString(this, string.Format("~/Areas/Flexpage/Views/Flexpage/NewsTemplates/{0}.cshtml", block.TemplateUrl), record);
        //            elementPos++;
        //        }

        //        if (addFinishBlock)
        //        {
        //            result += "<div finishBlock='" + blockID + "'></div>";
        //        }

        //    }
        //    return result;
        //}

        public ActionResult Search(string searchString)
        {
            var model = _flexpageService.CreateSearchModel(searchString);
            return View("~/Areas/Flexpage/Views/Flexpage/Search.cshtml", model);
        }

        [HttpPost]
        public ActionResult UpdateSearch(Flexpage.Service.DTO.SearchModel model)
        {
            ModelState.Clear();
            var newModel = _flexpageService.DoSearch(model.SearchString);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/SearchPartial.cshtml", newModel);
        }

        [HttpPost]
        public PartialViewResult UpdateMediaPlaylist(int id, string command, string parameters, string mediaType)
        {
            PartialViewResult pr = null;
            command = command ?? "";
            switch (command.ToLowerInvariant())
            {
                case "selectitem":
                    {
                        MediaModel mo = null;
                        if (mediaType == MediaTypeName.Audio.GetDisplay())
                        {
                            mo = CreateAndLoadModel(new BlockCommandModel(_settings) { BlockType = mediaType, ID = id, IsEditor = false }) as AudioModel;
                        }
                        else
                        {
                            mo = CreateAndLoadModel(new BlockCommandModel(_settings) { BlockType = mediaType, ID = id, IsEditor = false }) as VideoModel;
                        }

                        mo.Width.Type = SizeType.Full;
                        mo.Height.Type = SizeType.Full;
                        pr = PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/_" + mediaType + ".cshtml", mo);
                        break;
                    }
            }
            return pr;

        }

        public ActionResult galleryImage(string path)
        {
            return getImageForGallery(path, false);
        }

        public ActionResult galleryThumbImage(string path, string slider)
        {
            return getImageForGallery(path, true, slider == "1");
        }

        protected ActionResult getImageForGallery(string path, bool forThumb, bool slider = false)
        {
            string abspath = Server.MapPath(path);

            var thumbInfo = Helpers.GalleryHelper.ThumbsPrefixes.FirstOrDefault(tp => path.Contains(tp.Postfix));

            byte[] filedata = Helpers.GalleryHelper.GetFileData(Server, path, out string filename, out string contentType, thumbInfo, !slider && forThumb && thumbInfo == null, slider);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(path),
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(filedata, contentType);
        }

        public ActionResult getFileTile(int id, bool preview = false)
        {
            NamedStream namedStream = _fileConverter.GetTileBackground(id, preview);
            //prepare result
            //TODO replace user ID
            string contentType = MimeMapping.GetMimeMapping(namedStream.Name);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = namedStream.Name,
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(namedStream.Stream, contentType);
        }

        public ActionResult FolderContentDownloadZip(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return null;
            List<int> fileIDs = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt32(i)).ToList();

            NamedStream namedStream = _fileService.DownloadZip(fileIDs);
            string contentType = MimeMapping.GetMimeMapping(namedStream.Name);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = namedStream.Name,
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(namedStream.Stream, contentType);
        }


        [ValidateInput(false)]
        public ActionResult UploadControl_ProcessingFileUpload(string uploadControlName)
        {
            UploadControlExtension.GetUploadedFiles(uploadControlName, null, (s, e) =>
            {
                string folderPath = WebConfigurationManager.AppSettings["CMS:TempUploadedWebFormFile"];
                if (e.UploadedFile.IsValid)
                {
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
                    e.CallbackData = resultFileName;
                }
            });
            return null;
        }


        public ActionResult GetDayTitles(DateTime date)
        {
            var modelFromDb = _repository.GetEntityList<Event>()
                .Where(p => (p.StartDate.Date == date.Date.Date && p.IsPrimary)
                || date.Date.Date == p.StartDate.Date && !p.EndDate.HasValue
                || (date.Date >= p.StartDate.Date && p.EndDate.HasValue && date.Date <= p.EndDate.Value))
                .ToList();

            var models = new List<Flexpage.Models.EventModel>();

            foreach (var item in modelFromDb)
            {
                var eventModel = new EventModel(_settings, _flexpageProcessor);
                eventModel.Assign(item);
                models.Add(eventModel);
            }

            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/Calendar/CalendarToolTip.cshtml", models);
        }

        public ActionResult Disclaimer()
        {
            var model = new DisclaimerModel(_settings, _flexpageProcessor);
            model.Load(_repository, null);

            if (!model.Enabled)
                return null;

            return PartialView("~/Areas/Flexpage/Views/Shared/DisclaimerBanner.cshtml", model);
        }

        [HttpPost]
        public JsonResult FolderContent_UploadFile()
        {
            bool overwrite = false;
            var filesExist = new List<string>();
            try
            {
                if (!int.TryParse(Request.Params.Get("id"), out var blockID))
                {
                    throw new ArgumentNullException();
                }

                var selectFolderName = Request.Params.Get("selectFolderName");
                // Old root folder rules
                //if (selectFolderName == "\\")
                //{
                //    throw new Exception("Can't create a file in the root folder.");
                //}
                if (!string.IsNullOrEmpty(Request.Params.Get("overwrite")))
                {
                    overwrite = Request.Params.Get("overwrite").Trim().ToLower() == "true";
                }
                var destinationFolderName = Models.FolderContentModel.GetFolderName(_repository, blockID, selectFolderName);

                foreach (string file in Request.Files)
                {
                    var upload = Request.Files[file];
                    var strDT = Request.Params.Get(file + "_dateModification").ToString();
                    DateTime dateModification = DateTime.Parse(strDT);
                    if (upload != null)
                    {
                        var namedStream = new NamedStream()
                        {
                            Name = System.IO.Path.GetFileName(upload.FileName),
                            Stream = upload.InputStream,
                            Overwrite = overwrite,
                            DateModification = dateModification
                        };

                        if (_fileService.UploadFile(destinationFolderName, namedStream, "") == false)
                        {
                            filesExist.Add(upload.FileName);
                        }
                    }
                }
                if (filesExist.Count() > 0 && overwrite == false)
                {
                    throw new Exception(String.Join(", ", filesExist.ToArray()) + " already exists. Do you want to replace it?");
                }
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Upload File\",\"message\" : \"" + e.Message + "\",\"overwrite\" : " + (filesExist.Count > 0).ToString().ToLower() + "}}");
            }

            return Json("{\"success\": true}");
        }

        public ActionResult GetAdvertisement(int id, string tagName)
        {
            /* A block is "Static" if it loads all the images at the page load and doesnt call server for new images.
             * A static block:
             * 1) Doesnt get the models from the session (only "dynamic" blocks use session)
             * 2) Has a batch size equal (or higher) to the model count (otherwise its a dynamic block's first load)
             */
            bool isBlockStatic = true;
            List<AdvertisementModel> models;
            var sessionKey = "AdverisementModelsSequence" + id;

            if (Session[sessionKey] != null)
            {
                isBlockStatic = false;
                models = (List<AdvertisementModel>)Session[sessionKey];
            }
            else
            {
                models = AdvertisementModel.GenerateAdvertisementModels(new BlockCommandModel(_settings) { ID = id, Command = tagName }, _settings, _flexpageProcessor, _repository);
            }

            if (models.Count > 0)
            {
                var batchSize = models.First().ImagesPerLoad;
                if (batchSize < models.Count())
                    isBlockStatic = false;
                var rng = new Random();
                rng.Shuffle(models);

                var batch = models.Take(batchSize);

                var views = batch.Select(model =>
                    Flexpage.Helpers.ViewHelper.RenderPartialViewToString(
                        ControllerContext,
                        "~/Areas/Flexpage/Views/Flexpage/_AdvertisementPartial.cshtml",
                        model
                    )
                ).ToArray();

                var result = JsonConvert.SerializeObject(views);
                if (!isBlockStatic)
                {
                    if (models.Count() < batchSize)
                    {
                        models.Clear();
                    }
                    else
                    {
                        models.RemoveRange(0, batchSize);
                    };
                    if (models.Count > 0)
                    {
                        Session[sessionKey] = models;
                    }
                    else
                    {
                        Session.Remove(sessionKey);
                    }
                }
                else
                {
                    Session.Remove(sessionKey);
                }

                return Json(new { viewArray = result, allImagesLoaded = isBlockStatic });
            }

            return null;
        }

        [Authorize]
        public ActionResult FileHistoryGrid(int fileId)
        {
            var model = _fileService.LoadFileHistoryModel(fileId);

            return PartialView(
                "~/Areas/Flexpage/Views/Shared/EditorTemplates/FolderContent/_FileHistoryGrid.cshtml", model);
        }

        [Authorize]
        public ActionResult GetActualRevisionId(int fileId)
        {
            return Content(_fileService.GetFileVersionHistories(fileId).OrderByDescending(s => s.RevisionID)
                .Select(s => s.RevisionID)
                .FirstOrDefault().ToString());
        }

        [Authorize]
        public ActionResult RemoveFileHistory(int fileId, int revisionId)
        {
            _fileService.FileVersionRemove(fileId, revisionId);

            return null;
        }

        [Authorize]
        public ActionResult AddButtonControlToFileHistoryDialog()
        {
            ViewData["isPermission"] = _permissionsService.UserHasGeneralPermission(eGeneralPermission.ClearOldFileVersions) || _settings.IsCurrentPageAdmin();

            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FolderContent/_FileHistoryButtons.cshtml");
        }

        public JsonResult CheckLinkedObjectPropertyValues(int? propertyID)
        {
            var values = _repository.GetObjectPropertiesValues();
            if (values.Any(w => w.ObjectPropertyID == propertyID))
            {
                return Json("{\"linkedValues\": true}");
            }
            return Json("{\"linkedValues\": false}");
        }

        public ActionResult CustomProperties_EditTemplateHtml(CustomPropertiesEditTemplateHtmlModel model, string Name)
        {
            if (model == null)
            {
                model = new CustomPropertiesEditTemplateHtmlModel() { Name = Name };
            }
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/ContactDetails/_CustomPropertiesEditTemplateHtml.cshtml", model);
        }

        public ActionResult BrowserSelector()
        {
            return WrapBlockCreation("BrowserSelector", () =>
            {

                var model = new BrowserSelectorModel(_settings, _flexpageProcessor, User.Identity.Name);
                model.Load(_repository, new BlockCommandModel(_settings) { ID = 0 }, "Open a saved search query");
                model.FillViewData(ViewData, _repository, "Open a saved search query");
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Selectors/BrowserSelector.cshtml", model);
            });

        }
        // public ActionResult BrowserSelectorNew(int? selectedFileID = null, string title = "Select file")
        public ActionResult BrowserSelectorNew(BlockCommandModel model)
        {
            var title = "Select file";
            return WrapBlockCreation("BrowserSelector", () =>
            {

                var m = new BrowserSelectorModel(_settings, _flexpageProcessor, User.Identity.Name);
                m.Load(_repository, new BlockCommandModel(_settings) { ID = 0 }, title);
                // Following code must be placed into further browser provider 
                var prms = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.Parameters);
                if (prms.ContainsKey("selectedFileID"))
                {
                    var v = prms["selectedFileID"];
                    int selectedFileID;
                    if (int.TryParse(v, out selectedFileID))
                    {
                        var file = _fileService.GetFile(selectedFileID);
                        if (file != null)
                        {
                            var folderIDProp = file.Properties.FirstOrDefault(e => e.Name == "Folder.ID");
                            if (folderIDProp != null)
                            {
                                m.SelectedFileID = selectedFileID;
                                m.FolderTreeListModel.SelectedFolderID = (int)folderIDProp.Value < 0 ? null : (int?)folderIDProp.Value;
                            }
                        }
                    }
                }
                // end of code
                m.FillViewData(ViewData, _repository, title);
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Selectors/BrowserSelectorNew.cshtml", m);
            });
        }

        public ActionResult FolderSaveAsContactsSelector()
        {
            return WrapBlockCreation("FolderSaveAsContactsSelector", () =>
            {
                var model = new FolderSaveAsContactsSelectorModel(_settings, _flexpageProcessor, User.Identity.Name);
                model.Load(_repository, new BlockCommandModel(_settings) { ID = 0 }, "SAVE DIAGRAM", true, true);
                model.FillViewData(ViewData, _repository, "SAVE DIAGRAM");
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Selectors/FolderSaveAsContactsSelector.cshtml", model);
            });
        }

        private string TransformMultiSelectValue(string source)
        {
            StringBuilder sb = new StringBuilder();
            int val;
            if (source != null)
            {
                int i = 0;
                int lp = 0;
                string tstr;
                while (i < source.Length)
                {
                    lp = source.IndexOf('|', i);
                    if (lp >= 0)
                    {
                        tstr = source.Substring(i, lp - i);
                        if (int.TryParse(tstr, out val))
                        {
                            tstr = source.Substring(lp + 1, val);
                            i = lp + 1 + val;
                            if (int.TryParse(tstr, out val))
                            {
                                if (sb.Length > 0)
                                    sb.Append('+');
                                sb.Append(val);
                            }
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            return sb.ToString();
        }

        [HttpPost]
        public ActionResult UpdateObjectProperties(ObjectPropertiesModel model, string command, string parameters)
        {
            string updateCommand = command.ToLower().Trim();

            var admin = _settings.IsCmsAdmin();
            var canEditProperties =
                _permissionsService.UserHasGeneralPermission(eGeneralPermission.ManageFolderProperties);
            if (!admin && !canEditProperties)
            {
                return Json(
                    "{\"success\": false,\"error\": { \"title\" : \"Update object properties\",\"message\" : \"User doesn't have enough permissions\"}}");
            }

            if (updateCommand != "save")
                return GetEditorFor(model);

            var obj = _propertyService.GetObjectCustomPropertiesModel(model.ID);

            var property = _propertyService
                .EnumerateAllProperties()
                .Where(w => model.GetEditProperties().Properties.Where(n => n.Status == StatusPropertyEnum.New || n.Status == StatusPropertyEnum.Existing)
                    .Any(p => p.Id == w.ID)).ToList();

            property.ForEach(item =>
            {
                var value = model.GetEditProperties().Properties.FirstOrDefault(w => w.Id == item.ID)?.Value.ToString() ?? string.Empty;

                model.Properties.Properties.Add(new CustomPropertyInfo()
                {
                    ID = item.ID,
                    Name = item.Name,
                    Title = item.Caption,
                    Type = item.Type,
                    Value = item.Type == Pluritech.Properties.Abstract.DTO.ObjectPropertyType.Html ? HttpUtility.UrlDecode(value) : item.Type == Pluritech.Properties.Abstract.DTO.ObjectPropertyType.MultiselectEnum ? TransformMultiSelectValue(value) : value
                });
            });

            foreach (var customPropertyInfo in model.GetEditProperties().Properties.Where(n => n.Status == StatusPropertyEnum.Removed).ToList())
            {
                model.Properties.ToDelete.Add(customPropertyInfo.Id);
            }

            //save permissions first because other saving can be ralated on them
            if (admin)
            {
                _permissionsService.SaveObjectPermissions(model.Permissions);
                if (model.Type != ObjectPropertiesTypeEnum.ContactsShortcut)
                {
                    if (string.IsNullOrEmpty(parameters))
                        _fileService.SetFolderVersioned(model.ID, model.IsSupportVersion, model.SupportVersion,
                            model.IsRecursive);
                    else
                        _fileService.SetFileVersioned(model.ID, model.IsSupportVersion, model.SupportVersion);
                }

            }

            _propertyService.SaveObjectCustomPropertiesValues(model.Properties);

            return Json("{\"success\": true, \"title\" : \"Object Properties\",\"message\" : \"Object Properties updated successfully\"}");
        }

        [HttpPost]
        public PartialViewResult ShowAllAvailableProperties(CustomPropertyModel model, bool VisibleEnabled, bool LoadModel = false)
        {
            if (LoadModel)
            {
                model = _propertyService.GetObjectCustomPropertiesModel(model.ObjectID);
            }
            var props = _propertyService.GetObjectCustomPropertiesModel(model.ObjectID);

            props.VisibleEnabled = VisibleEnabled;

            if (VisibleEnabled)
            {
                props.AllowedProperties.ToList().ForEach(p =>
                {
                    var current = props.Properties.FirstOrDefault(c => c.ID == p.ID);
                    if (current == null)
                    {
                        props.Properties.Add(new CustomPropertyInfo()
                        {
                            ID = p.ID,
                            Name = p.Name,
                            Title = p.Caption,
                            Value = string.Empty,
                            DisplayValue = string.Empty,
                            Enabled = false,
                            Type = p.Type
                        });
                    }
                });
            }
            props.Properties.ForEach(p =>
            {
                var current = model.Properties.FirstOrDefault(c => c.ID == p.ID);
                if (current != null)
                {
                    p.Enabled = true;
                    if (p.Type == Pluritech.Properties.Abstract.DTO.ObjectPropertyType.File)
                    {
                        int v = 0;
                        if (int.TryParse(current.DisplayValue, out v))
                        {
                            var fi = _fileService.GetFile(v).Properties.FirstOrDefault(e => e.Name == "Name");
                            if (fi != null)
                                p.DisplayValue = fi.Value as string;
                        }
                        else
                            p.DisplayValue = "";
                    }
                    else
                        p.DisplayValue = p.Type == Pluritech.Properties.Abstract.DTO.ObjectPropertyType.Html ? HttpUtility.UrlDecode(current.DisplayValue) : current.DisplayValue;
                }
            });
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/_BrowserCustomPropertiesGrid.cshtml", props);
        }

        #region Custom property
        public ActionResult CustomPropertyAddEditors(string propertiesRequest)
        {
            var model = new CustomPropertyAddModel(_settings, _flexpageProcessor);

            model.ConvertData(propertiesRequest);

            var obj = _propertyService.GetObjectCustomPropertiesModel(model.PropertiesRequest.ObjectId);
            var selectIds = new List<int>();

            if (model.PropertiesRequest?.Properties.Count > 0)
            {
                selectIds = model.PropertiesRequest.Properties
                    .Where(w => w.Status == StatusPropertyEnum.Existing || w.Status == StatusPropertyEnum.New)
                    .Select(s => s.Id).ToList();
            }

            obj.AllowedProperties.Where(w => !selectIds.Contains(w.ID)).ToList().ForEach(
                item =>
                {
                    model.SelectListItems.Add(new SelectListItem()
                    {
                        Value = item.ID.ToString(),
                        Text = item.Caption,
                    });
                });
            obj.LangCode = _localization.GetCurrentOrDefaultLangCode();
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/CustomPropertyAddEditors.cshtml", model);

        }

        public ActionResult AddCustomProperty(string propertiesRequest)
        {
            var model = new CustomPropertyAddModel(_settings, _flexpageProcessor);

            model.ConvertData(propertiesRequest);

            var obj = _propertyService.GetObjectCustomPropertiesModel(model.PropertiesRequest.ObjectId);
            var ids = model.PropertiesRequest.Properties.Where(w => w.Status == StatusPropertyEnum.New).Select(s => s.Id).ToList();
            var data = _propertyService
                .EnumerateAllProperties()
                .Where(w => ids.Contains(w.ID) && !obj.Properties.Select(s => s.ID).Contains(w.ID)).ToList();

            data.ForEach(
                item =>
                {
                    obj.Properties.Add(new CustomPropertyInfo()
                    {
                        ID = item.ID,
                        Name = item.Name,
                        Title = item.Caption,
                        Type = item.Type,
                        DisplayValue = model.PropertiesRequest.Properties.FirstOrDefault(w => w.Id == item.ID)?.Value.ToString() ?? string.Empty
                    });
                });

            ids = model.PropertiesRequest.Properties.Where(w => w.Status == StatusPropertyEnum.Removed).Select(s => s.Id).ToList();
            obj.Properties = obj.Properties.Where(w => !ids.Contains(w.ID)).ToList();

            obj.Properties.ForEach(item =>
            {
                item.DisplayValue = model.PropertiesRequest.Properties.FirstOrDefault(w => w.Id == item.ID)?.Value.ToString() ?? string.Empty;
            });

            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/_BrowserCustomPropertiesGrid.cshtml", obj);
        }

        public ActionResult CustomProperty_ShowEditDialog(string typeProperty, int objectPropertyID, int folderID)
        {
            var model = new PublishingFolderModel();
            model.Load(_repository, objectPropertyID, folderID);

            return View($"~/Areas/Flexpage/Views/Flexpage/Editors/CustomProperty/_{typeProperty}.cshtml", model);
        }
        public ActionResult CustomProperty_ShowHtml(string typeProperty, int objectID, int objectPropertyID)
        {
            var props = _propertyService.GetObjectCustomProperties(objectID);
            var prop = props.FirstOrDefault(p => p.ID == objectPropertyID);

            return View($"~/Areas/Flexpage/Views/Flexpage/Editors/_CustomProperty_Html.cshtml",
                new Flexpage.Models.CustomPropertiesEditTemplateHtmlModel() { Value = prop?.Value ?? "", ObjectPropertyID = objectID, PropertyID = objectPropertyID });
        }
        public ActionResult CustomProperties_SaveHtml(CustomPropertiesEditTemplateHtmlModel model, int folderID)
        {
            return Json("{\"success\": true, \"title\" : \"Save Custom Properties Html Object Types\",\"message\" : \"Object Types saved successfully\"}");
        }

        public ActionResult UpdatePublishingFolderObjectProperties(PublishingFolderModel publishingFolderModel)
        {
            publishingFolderModel.Save(_repository);
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/CustomProperty/_PublishingFolderGrid.cshtml", publishingFolderModel);
        }

        public ActionResult CustomProperty_WebSiteGrid(int objectPropertyID, int folderID)
        {
            var model = new PublishingFolderModel();
            model.Load(_repository, objectPropertyID, folderID);

            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/CustomProperty/_PublishingFolderGrid.cshtml", model);
        }
        #endregion
    }

    public class MyLogger
    {
        public static void Log(string text)
        {
            // Path for CC prod server
            // System.IO.File.AppendAllText("C:\\inetpub\\wwwroot\\CopaCogeca5\\App_Data\\cc-page-log.txt", $"{DateTime.Now.ToString("mm:ss:fff")} {text}\n");
            System.IO.File.AppendAllText("c:\\Temp\\flexpage-log.txt", $"{DateTime.Now.ToString("mm:ss:fff")} {text}\n");
        }
    }

}