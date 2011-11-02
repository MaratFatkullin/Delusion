﻿using System;
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
            string url)
        {
            var attrs = new Dictionary<string, object> { { "data-autocomplete-source-path", url } };
            var textBoxHtmlString = htmlHelper.TextBoxFor(expression, attrs);
            var str = string.Format("<span>{0}</span>", textBoxHtmlString);
            return MvcHtmlString.Create(str);
        }
    }
}