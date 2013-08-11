using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;

namespace Softball.Mvc4.Helpers
{
    public static class CheckBoxListHelper
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo, bool addSelectAll = false)
        {
            return htmlHelper.CheckBoxList(name, listInfo, addSelectAll, ((IDictionary<string, object>)null));
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo, bool addSelectAll, object htmlAttributes)
        {
            return htmlHelper.CheckBoxList(name, listInfo, addSelectAll, ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo, bool addSelectAll, IDictionary<string, object> htmlAttributes)
        {
            // Validate incoming arguments
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("The argument must have a value", "name");
            if (listInfo == null)
                throw new ArgumentNullException("listInfo");
            if (listInfo.Count < 1)
                throw new ArgumentException("The list must contain at least one value", "listInfo");

            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");

            if (addSelectAll)
            {
                TagBuilder builder = new TagBuilder("input");

                builder.MergeAttributes<string, object>(htmlAttributes);
                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", "");
                builder.MergeAttribute("name", name + "_all");
                builder.InnerHtml = " (select all)";
                sb.Append("<li>");
                sb.Append(builder.ToString(TagRenderMode.Normal));
                sb.Append("</li>");
            }

            foreach (CheckBoxListInfo info in listInfo)
            {
                TagBuilder builder = new TagBuilder("input");

                if (info.IsChecked) builder.MergeAttribute("checked", "checked");

                builder.MergeAttributes<string, object>(htmlAttributes);
                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", info.Value);
                builder.MergeAttribute("name", name);
                builder.InnerHtml = " " + info.DisplayText;
                sb.Append("<li>");
                sb.Append(builder.ToString(TagRenderMode.Normal));
                sb.Append("</li>");
            }

            sb.Append("</ul>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }

    // This the information that is needed by each checkbox in the
    // CheckBoxList helper.
    public class CheckBoxListInfo
    {
        public CheckBoxListInfo() {}

        public CheckBoxListInfo(string value, string displayText, bool isChecked)
        {
            this.Value = value;
            this.DisplayText = displayText;
            this.IsChecked = isChecked;
        }

        public string Value { get; set; }
        public string DisplayText { get; set; }
        public bool IsChecked { get; set; }
    }
}