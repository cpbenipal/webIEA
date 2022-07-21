using System;
using System.IO;
using System.Web.Mvc;

namespace Flexpage.Abstract
{
    public interface IViewConverter
    {
        string RenderPartialViewToString(Controller controller, string viewName, object model);
        string RenderPartialViewToString(Controller controller, PartialViewResult viewResult);
        string RenderPartialViewToString(ControllerContext controllerContext, string viewName, object model);
    }
}
