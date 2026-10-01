using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MTAoarsGeneral.Utilities.Mvc
{

    public static class HtmlHelperPathExtensions
    {

        public static MvcHtmlString Script(this HtmlHelper helper, string path)
        {
            var tb = new TagBuilder("script");
            tb.MergeAttribute("type", "text/javascript");
            tb.MergeAttribute("src", UrlHelper.GenerateContentUrl(path, helper.ViewContext.HttpContext));
            return MvcHtmlString.Create(tb.ToString());
        }

        public static MvcHtmlString StyleSheet(this HtmlHelper helper, string path)
        {
            var tb = new TagBuilder("link");
            tb.MergeAttribute("rel", "stylesheet");
            tb.MergeAttribute("media", "all");
            tb.MergeAttribute("href",
                UrlHelper.GenerateContentUrl(path, helper.ViewContext.HttpContext)
                //helper.ToAbsolute(path)
                );
            return MvcHtmlString.Create(tb.ToString());
        }

        public static string ToAbsolute(this HtmlHelper helper, string relativeUrl)
        {
            var request = helper.ViewContext.HttpContext.Request;
            return string.Format("http{0}://{1}{2}",
                (request.IsSecureConnection) ? "s" : "",
                request.Url.Authority,
                UrlHelper.GenerateContentUrl(relativeUrl, helper.ViewContext.HttpContext)
            );
        }

    }

}
