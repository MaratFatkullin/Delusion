using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AI_.Studmix.WebApplication.Helpers
{
    public static class HtmlHelperExtension
    {
        public static MvcHtmlString CombBoxFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string url,
            bool isReadOnly = false)
        {
            var htmlAttributes = new Dictionary<string, object> {{"data-autocomplete-source-path", url}};
            if (isReadOnly)
                htmlAttributes.Add("readonly", "readonly");
            var textBoxHtmlString = htmlHelper.TextBoxFor(expression, htmlAttributes);
            var str = string.Format("<span>{0}</span>", textBoxHtmlString);
            return MvcHtmlString.Create(str);
        }

        public static MvcHtmlString DisabledTextBoxFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var htmlAttributes = new Dictionary<string, object> {{"disabled", "disabled"}};
            var textBoxHtmlString = htmlHelper.TextBoxFor(expression, htmlAttributes);
            return textBoxHtmlString;
        }
    }
}