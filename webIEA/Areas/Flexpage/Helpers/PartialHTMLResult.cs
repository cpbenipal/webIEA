using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexPage.Helpers
{
    public class PartialHTMLResult : PartialViewResult
    {

        //поговорить завтра с Лехой о том, что кастомный экшен и редирект  - суть одно и тоже, только в кастомный экшн постятся параметры
        //нам нужен еще один экшн - GlobalCustomAction (или InplaceCustomAction)
        public string HTML { get; set; }
        public PartialHTMLResult(string html)
        {
            HTML = html;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write(HTML);
        }
    }
}