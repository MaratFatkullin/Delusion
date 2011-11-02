using System.Web;
using System.Web.Mvc;

namespace AI_.Studmix.WebApplication.Helpers
{
    public static class UrlHelperExtension
    {
        public static HtmlString Script(this UrlHelper urlHelper, string filename)
        {
            var markup = string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>",
                                       urlHelper.Content("~/Scripts/" + filename));
            return new HtmlString(markup);
        }
    }
}