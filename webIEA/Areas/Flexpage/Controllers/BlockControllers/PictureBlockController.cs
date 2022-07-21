using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class PictureBlockController : Controller
    {
        private readonly IFlexpageRepository _repository;
        private readonly IFlexpageSettings _settings;
        private readonly IFlexpage _flexpageProcessor;

        public PictureBlockController(IFlexpageRepository repository, IFlexpageSettings settings, IFlexpage flexpageProcessor) 
        {
            _flexpageProcessor = flexpageProcessor;
            _repository = repository;
            _settings = settings;
        }

        public PartialViewResult Picture(string alias)
        {
            try
            {
                ViewModel m = ViewModel.Create("PictureModel", _settings, _flexpageProcessor);
                m.Load(_repository, new BlockCommandModel(_settings) { BlockAlias = alias, ID = Models.BlockModel.NewStaticBlockID });
                m.IsStatic = true;

                return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
            }
#if DEBUG
            catch (Exception ex)
            {
                return PartialView("~/Areas/Flexpage/Views/Shared/Error.cshtml", String.Format("Sorry, there is something wrong with block '{0}'. Please contact your administrator. <br><b>Message:</b> {1} <br><b>Stacktrace:</b> {2}", alias, ex.Message, ex.StackTrace));
#else
            catch
            {
#endif
                return PartialView("~/Areas/Flexpage/Views/Shared/Error.cshtml", String.Format("Sorry, there is something wrong with block '{0}'. Please contact your administrator.", alias));
            }
        }
    }
}