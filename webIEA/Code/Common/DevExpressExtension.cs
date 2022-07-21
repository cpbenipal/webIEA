using DevExpress.Web.Internal;
using System.Web.Mvc;

namespace DevExpress.Web.Mvc.UI
{
    // This is required to render devexpress resources in different views
    // Source: https://supportcenter.devexpress.com/ticket/details/t106544/multiple-calls-of-the-getscripts-method-cause-the-uncaught-rangeerror-maximum-call-stack
    public static class ExtensionsFactoryExtension
    {
        public static MvcHtmlString GetScriptsEx(this ExtensionsFactory factory, params Script[] scriptItems)
        {
            try
            {
                return factory.GetScripts(scriptItems);
            }
            finally
            {
                MvcUtils.RenderScriptsCalled = false;
            }
        }
    }
}