using Flexpage.Code.StructureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;
using System.Text;

namespace Flexpage.Code.Helpers
{
    public class SiteMapTreeView : IHtmlString
    {
        private readonly HtmlHelper _html;
        private readonly TreeList _treeList = null;
        private readonly string _id = "";
        private readonly bool _forManager = false;

        int treeItemCount = 0;
        public SiteMapTreeView(HtmlHelper html, string id, TreeList treeList, bool forManager = false)
        {
            if (html == null) throw new ArgumentNullException("html");
            _html = html;
            _id = id;
            _treeList = treeList;
            _forManager = forManager;
        }

        public string ToHtmlString()
        {
            return ToString();
        }

        public void Render()
        {
            var writer = _html.ViewContext.Writer;
            using (var textWriter = new HtmlTextWriter(writer))
            {
                textWriter.Write(ToString());
            }
        }


        public override string ToString()
        {
            if (_treeList == null)
            {
                return "";
            }

            var div = new TagBuilder("div");
            if (string.IsNullOrEmpty(_id) == false)
            {
                div.Attributes.Add("id", _id);
            }
            div.Attributes.Add("class", "clearfix" + (_forManager == false ? " page-sitemaps sitemap-viewer" : ""));

            TagBuilder listContainer = new TagBuilder("div");
            listContainer.AddCssClass("pages-list all-scroll ui-sortable");
            listContainer.Attributes.Add("id", "menuSortable");
            div.InnerHtml += listContainer.ToString(TagRenderMode.StartTag);
            if (_treeList.Root.Nodes.Length > 0)
            {
                int itemOrder = 0;
                foreach (var item in _treeList.Root.Nodes)
                {
                    buildNestedTagWithHeader(listContainer, item, itemOrder, true);
                    itemOrder++;

                }
            }

            insertScript(listContainer);
            div.InnerHtml += listContainer.InnerHtml + listContainer.ToString(TagRenderMode.EndTag);
            return div.ToString();
        }

        private void appendChildren(TagBuilder parentTag, TreeNode parentItem)
        {
            var children = parentItem.Nodes;
            if (!children.Any())
            {
                return;
            }
            int itemOrder = 0;
            foreach (var item in children)
            {
                buildNestedTagWithHeader(parentTag, item, itemOrder);
                itemOrder++;
            }
        }

        private void buildNestedTagWithHeader(TagBuilder parentTag, TreeNode parentItem, int itemOrder, bool firstLevel = false)
        {
            treeItemCount++;
            StringBuilder builder = new StringBuilder();
            string css = String.Empty;
            if (!firstLevel)
            {
                if (parentItem.Nodes != null && parentItem.Nodes.Count() > 0)
                {
                    builder = new StringBuilder("<div itemorder=" + itemOrder + " class='pages-list ui-state-default all-scroll ui-sortable-handle'> " +
                 
                                "<div class='panel-heading  panel-page'>" +
                                    "<h4 class='panel-title'>" +
                                        "<a class='anchor-title anchor-collapse sitemap-item' data-toggle='collapse' href='#" + (_forManager == false ? "view" : "") + "collapse" + treeItemCount + "' " + (string.IsNullOrEmpty(_id) == false ? (" data-parent='#" + _id + "'") : "") + "  aria-expanded='false' aria-controls='collapse" + treeItemCount + "' data-resourceKey='" + parentItem.Key + "' > " + parentItem.Title + "<span class='pull-right'><i class='fas fa-chevron-down rotate'></i></span></a>" +
                                    "</h4>" +
                                "</div>");
                }
                else
                {
                    builder = new StringBuilder("<div itemorder=" + itemOrder + " class='pages-list ui-state-default all-scroll ui-sortable-handle'>" +
                                "<div class='panel-heading  panel-page'>" +
                                    "<h4 class='panel-title'>" +
                                        "<a class='anchor-title anchor-collapse sitemap-item'" + (string.IsNullOrEmpty(_id) == false ? (" data-parent='#" + _id + "'") : "") + "  aria-controls='collapse" + treeItemCount + "' data-resourceKey='" + parentItem.Key + "' > " + parentItem.Title + "</a>" +
                                    "</h4>" +
                                "</div>");
                }
            }
            else
            {
                if (parentItem.Nodes != null && parentItem.Nodes.Count() > 0)
                {
                    builder = new StringBuilder("<div itemorder=" + itemOrder + " class='ui-state-default all-scroll ui-sortable-handle'> " +
                            "<div class='panel panel-default'>" +
                                "<div class='panel-heading  panel-page'>" +
                                    "<h4 class='panel-title'>" +
                                        "<a class='anchor-title anchor-collapse sitemap-item' data-toggle='collapse' href='#" + (_forManager == false ? "view" : "") + "collapse" + treeItemCount + "' " + (string.IsNullOrEmpty(_id) == false ? (" data-parent='#" + _id + "'") : "") + "  aria-expanded='false' aria-controls='collapse" + treeItemCount + "' data-resourceKey='" + parentItem.Key + "' > " + parentItem.Title + "<span class='pull-right'><i class='fas fa-chevron-down rotate'></i></span></a>" +
                                    "</h4>" +
                                "</div>");
                }
                else
                {
                    builder = new StringBuilder("<div itemorder=" + itemOrder + " class='ui-state-default all-scroll ui-sortable-handle'>" +
                            "<div class='panel panel-default'>" +
                                "<div class='panel-heading  panel-page'>" +
                                    "<h4 class='panel-title'>" +
                                        "<a class='anchor-title anchor-collapse sitemap-item'" + (string.IsNullOrEmpty(_id) == false ? (" data-parent='#" + _id + "'") : "") + "  aria-controls='collapse" + treeItemCount + "' data-resourceKey='" + parentItem.Key + "' > " + parentItem.Title + "</a>" +
                                    "</h4>" +
                                "</div>");
                }
            }

    TagBuilder listContainer = new TagBuilder("div");
            listContainer.MergeAttribute("id", (_forManager == false ? "view" : "") + "collapse" + treeItemCount);
            listContainer.AddCssClass("panel-collapse collapse collapse-field-container");

            builder.Append(listContainer.ToString(TagRenderMode.StartTag));
            appendChildren(listContainer, parentItem);
            builder.Append(listContainer.InnerHtml + listContainer.ToString(TagRenderMode.EndTag));

            builder.Append("</div>");

            if (firstLevel)
            {
                builder.Append("</div>");
            }
            parentTag.InnerHtml += builder.ToString();
            
        }

        private void buildNestedTagWithoutHeader(TagBuilder parentTag, TreeNode parentItem)
        {
            parentTag.InnerHtml += parentItem.Title;
        }

        private void insertScript(TagBuilder parentTag)
        {
            var builder = new TagBuilder("script");

            builder.MergeAttribute("type", "text/javascript");
            //builder.InnerHtml+= "jQuery(document).ready(function(){" +
            //     "jQuery('#" + _id + "').find('h4.panel-title a').on('click',function(event){jQuery(this).toggleClass('fa-chevron-up fa-chevron-down');});" +
            //    "jQuery('#" + _id + "').find('a.initadd').on('click',function(event){SitemapManagerAddNew(this);});" +
            //    "jQuery('#" + _id + "').find('a.remove').on('click',function(event){SitemapManagerRemove(this);});" +
            //    "});";

            parentTag.InnerHtml += builder.ToString(TagRenderMode.Normal);
        }
    }

}