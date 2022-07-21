using Flexpage.Code.StructureManagement;
using System.Web.Mvc;

namespace Flexpage.Code.Helpers
{
    public enum JavascriptPathType
    {
        Relative = 0,
        FullWebPath = 1
    };

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Script(this HtmlHelper helper, string jsFileName, JavascriptPathType pathType)
        {
            // Instantiate a UrlHelper
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            // Create tag builder
            var builder = new TagBuilder("script");

            switch(pathType)
            {
                case JavascriptPathType.Relative:
                    builder.MergeAttribute("src", urlHelper.Content("~/Scripts/" + jsFileName));
                    break;
                case JavascriptPathType.FullWebPath:
                    builder.MergeAttribute("src", urlHelper.Content(jsFileName));
                    break;
            }

            // Add attributes
            builder.MergeAttribute("type", "text/javascript");

            // Render tag. Note: script tags have to have an End tag and cannot be self-closing.
            return new MvcHtmlString(builder.ToString(TagRenderMode.Normal));
        }

        /// Create an HTML tree from a recursive collection of items
        /// </summary>
        public static MvcHtmlString SiteMapTreeView(this HtmlHelper html, string id, TreeList treeList, bool forManager)
        {
            return MvcHtmlString.Create(new SiteMapTreeView(html, id, treeList, forManager).ToHtmlString());
        }
    }
}