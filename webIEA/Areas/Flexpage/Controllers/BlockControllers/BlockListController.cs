using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Flexpage.Abstract;
using Flexpage.Models;
using Flexpage.Domain.Abstract;


namespace Flexpage.Controllers
{
    public class BlockListController : Controller
    {
        private readonly IFlexpage _flexpageProcessor;
        private readonly IFlexpageSettings _settings;
        protected IFlexpageRepository _repository; // todo - remove it from here

        public ActionResult WrapBlockCreation(string alias, PartialViewDelegate viewCreationRoutine)
        {
            try
            {
                return viewCreationRoutine();
            }
            catch (Exception ex)
            {
#if DEBUG
                return PartialView("~/Areas/Flexpage/Views/Shared/Error.cshtml", String.Format("Sorry, there is something wrong with blocklist '{0}'. Please contact your administrator. <br><b>Message:</b> {1} <br><b>Stacktrace:</b> {2}", alias, ex.Message, ex.StackTrace));
#else
                return PartialView("~/Areas/Flexpage/Views/Shared/Error.cshtml", String.Format("Sorry, there is something wrong with blocklist '{0}'. Please contact your administrator.", alias));
#endif
            }

        }



        public BlockListController(IFlexpage flexpage, IFlexpageSettings settings, IFlexpageRepository repository)
        {
            _flexpageProcessor = flexpage;
            _settings = settings;
            _repository = repository;
        }

        // GET: Flexpage/BlockList
        public ActionResult Index()
        {
            return View();
        }

        #region Infrastructure
        [HttpGet]
        public ActionResult BlockListByID(int id)
        {
            return WrapBlockCreation(id.ToString(), () =>
            {
                var model = new BlockListModel(_settings, _flexpageProcessor);
                model.Load(_repository, new BlockCommandModel(_settings, _flexpageProcessor) { BlocklistID = id });
                return View("~/Areas/Flexpage/Views/Flexpage/BlockList.cshtml", model);
            });
        }
        [HttpGet]
        public PartialViewResult BlockList(string alias)
        {
            return WrapBlockCreation(alias, () => {
                var model = new BlockListModel(_settings, _flexpageProcessor);
                model.Load(_repository, new BlockCommandModel(_settings, _flexpageProcessor) { BlockAlias = alias });
                ViewBag.alias = alias;
                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockList.cshtml", model);
            }) as PartialViewResult; 
        }
        #endregion


    }
}