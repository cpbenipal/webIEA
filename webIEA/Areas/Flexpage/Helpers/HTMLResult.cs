using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexPage.Helpers
{
    public class HTMLResult : ActionResult
    {
        public string HTML { get; set; }
        public HTMLResult(string html)
        {
            HTML = html;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write(HTML);
        }
    }
}