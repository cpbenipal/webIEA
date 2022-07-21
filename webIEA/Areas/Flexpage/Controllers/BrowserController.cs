using DevExpress.Web.Mvc;
using Pluritech.Pluriworks.Service.Abstract;
using Pluritech.Pluriworks.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class BrowserController : Controller
    {
        private readonly IFileSearchProcessor _fileSearchProcessor;
        // GET: Flexpage/BrowserSearch
        public BrowserController(IFileSearchProcessor fileSearchProcessor)
        {
            _fileSearchProcessor = fileSearchProcessor;
        }
        public ActionResult BrowserSearch(BrowserSearchResultModel model)
        {
            try
            {
                if (!model.InContent && !model.InCustomFields && !model.InFileFolderName)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    throw new Exception("Select at least one search parameter");
                }
                if (string.IsNullOrWhiteSpace(model.SearchQuery))
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    throw new Exception("Search query cannot be empty");
                }
                model.SearchID = _fileSearchProcessor.StartSearch(model);
                return PartialView("~/Areas/Flexpage/Views/Flexpage/BrowserSearchResult.cshtml", model);
            }
            catch (Exception e)
            {
                if (Response.StatusCode != (int)HttpStatusCode.BadRequest)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                return Json(e.Message);
            }
        }

        public ActionResult BrowserSearchResultGrid(string searchQuery, bool inContent, bool inCustomFields, bool inFileFolderName, int tabIndex, int blockID, int searchID)
        {
            BrowserSearchResultModel model = new BrowserSearchResultModel()
            {
                TabIndex = tabIndex,
                SearchQuery = searchQuery,
                InContent = inContent,
                InCustomFields = inCustomFields,
                InFileFolderName = inFileFolderName,
                BlockID = blockID,
                SearchID = searchID
            };

            model.Status = _fileSearchProcessor.GetBrowserSearchStatus(searchID);

            model.GridModel.ProcessCustomBinding(
                e =>
                {
                    _fileSearchProcessor.GetDataObjectsCount(e, searchID);
                },
                e =>
                {
                    _fileSearchProcessor.GetDataObjects(e, searchID);
                });
            return PartialView("~/Areas/Flexpage/Views/Flexpage/BrowserSearchResultGrid.cshtml", model);

        }
        private ActionResult BrowserSearch_GridCoreAction(GridViewModel gridViewModel, string searchQuery, bool inContent, bool inCustomFields, bool inFileFolderName, int blockID, int tabIndex, int searchID)
        {
            BrowserSearchResultModel model = new BrowserSearchResultModel()
            {
                TabIndex = tabIndex,
                SearchQuery = searchQuery,
                InContent = inContent,
                InCustomFields = inCustomFields,
                InFileFolderName = inFileFolderName,
                BlockID = blockID,
                SearchID = searchID
            };

            model.GridModel = gridViewModel;

            model.GridModel.ProcessCustomBinding(
                e =>
                {
                    _fileSearchProcessor.GetDataObjectsCount(e, searchID);
                },
                e =>
                {
                    _fileSearchProcessor.GetDataObjects(e, searchID);
                });
            return PartialView("~/Areas/Flexpage/Views/Flexpage/BrowserSearchResultGrid.cshtml", model);
        }

        public ActionResult BrowserSearch_PagingAction(GridViewPagerState pager, string searchQuery, bool inContent, bool inCustomFields, bool inFileFolderName, int blockID, int tabIndex, int searchID, string name)
        {
            var viewModel = GridViewExtension.GetViewModel(name);
            viewModel.ApplyPagingState(pager);

            return BrowserSearch_GridCoreAction(viewModel, searchQuery, inContent, inCustomFields, inFileFolderName, blockID, tabIndex, searchID);
        }

        public ActionResult BrowserSearch_SortingAction(GridViewColumnState column, string searchQuery, bool inContent, bool inCustomFields, bool inFileFolderName, int blockID, int tabIndex, int searchID, string name)
        {
            var viewModel = GridViewExtension.GetViewModel(name);
            viewModel.SortBy(column, true);

            return BrowserSearch_GridCoreAction(viewModel, searchQuery, inContent, inCustomFields, inFileFolderName, blockID, tabIndex, searchID);
        }

        public ActionResult BrowserSearch_FilteringAction(GridViewFilteringState filteringState, string searchQuery, bool inContent, bool inCustomFields, bool inFileFolderName, int blockID, int tabIndex, int searchID, string name)
        {
            var viewModel = GridViewExtension.GetViewModel(name);
            viewModel.ApplyFilteringState(filteringState);

            return BrowserSearch_GridCoreAction(viewModel, searchQuery, inContent, inCustomFields, inFileFolderName, blockID, tabIndex, searchID);
        }

    }
}