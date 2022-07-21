using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexPage.Controllers
{
    public class LogController : Controller
    {
        ILogProvider _logProvider;
        public LogController(ILogProvider logProvider)
        {
            _logProvider = logProvider;
        }
        // GET: Flexpage/Log
        public ActionResult Index()
        {
            return View(@"~\Areas\Flexpage\Views\Log\Index.cshtml");
        }

        public PartialViewResult Logs()
        {
            var log = _logProvider.Create();
            return PartialView(@"~\Areas\Flexpage\Views\Log\Logs.cshtml", log);
        }

        [HttpPost]
        public ActionResult Logs_Callback(LogsModel model, string command, string parameters)//, string command, string parameters)
        {
            ModelState.Clear();
            try
            {
                _logProvider.SetTargetType(model);
                _logProvider.Load(model);
                model.Columns.ForEach(c => c.ColumnCaption = Resources.Strings.ResourceManager.GetString("LogEntry_" + c.Name) ?? c.ColumnCaption);

                return PartialView("~/Areas/Flexpage/Views/Log/Logs.cshtml", model);
            }
            catch
            {
                return View("~/Areas/Flexpage/Views/Log/Logs.cshtml", model);
            }
        }

    }
}