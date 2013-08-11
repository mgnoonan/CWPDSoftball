using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Softball.Mvc4.Helpers
{
    public static class HeadTagHelpers
    {
        public static string Link(this HtmlHelper helper, string url, string rel, string type)
        {
            return Link(helper, url, rel, type, string.Empty);
        }

        public static string Link(this HtmlHelper helper, string url, string rel, string type, string media)
        {
            // Instantiate a UrlHelper    
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            // Create tag builder   
            var builder = new TagBuilder("link");

            // Create valid id   
            //builder.GenerateId(id);

            // Add attributes   
            builder.MergeAttribute("href", urlHelper.Content(url));
            builder.MergeAttribute("rel", rel);
            builder.MergeAttribute("type", type);

            if (!string.IsNullOrEmpty(media))
                builder.MergeAttribute("media", media);

            // Render tag   
            return builder.ToString(TagRenderMode.SelfClosing);
        }

        public static string Script(this HtmlHelper helper, string url, string type)
        {
            // Instantiate a UrlHelper    
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            // Create tag builder   
            var builder = new TagBuilder("script");

            // Create valid id   
            //builder.GenerateId(id);

            // Add attributes   
            builder.MergeAttribute("href", urlHelper.Content(url));
            builder.MergeAttribute("type", type);

            // Render tag   
            return builder.ToString(TagRenderMode.SelfClosing);
        }
    }
}
