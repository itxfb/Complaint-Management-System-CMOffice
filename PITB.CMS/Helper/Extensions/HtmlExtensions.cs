using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace PITB.CMS.Helper.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> ex, IEnumerable<SelectListItem> values, object htmlAttributes)
        {
            string name = ExpressionHelper.GetExpressionText(ex);
            var value = ModelMetadata.FromLambdaExpression(ex, htmlHelper.ViewData).Model;
            //id.Na
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var sb = new StringBuilder();
            int counter = 1;

            //new { id = name + counter,@class=htmlAttributes}));
            foreach (var item in values)
            {
                //dynamic a = new ExpandoObject();
                //foreach (KeyValuePair<object,string> atr in htmlAttributes)
                //{
                //    var propName = atr.Key;
                //    var propValue = atr.Value;
                //    //a.
                //}
                object control;
                if (value != null)
                {
                    if (value is Enum)
                    {
                        if (item.Value == Convert.ToInt32(value).ToString())
                        {
                            attrs.Add("checked", true);

                        }
                        else
                            attrs.Remove("checked");
                    }
                    else if (item.Value == value.ToString())
                    {
                        attrs.Add("checked", true);


                    }
                    else
                    {

                        attrs.Remove("checked");
                    }
                }
                control = htmlHelper.RadioButtonFor(ex, item.Value, attrs);
                sb.Append(control);
                var label = new TagBuilder("label");
                label.SetInnerText(item.Text);
                label.Attributes.Add("for", name + counter);
                sb.Append(label);
                counter++;
            }
            return MvcHtmlString.Create(sb.ToString());
        }
        public static HtmlString GetEnums<T>(this HtmlHelper helper) where T : struct
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("if(!window.Enum) Enum = {};");
            var enumeration = Activator.CreateInstance(typeof(T));
            var enums = typeof(T).GetFields().ToDictionary(x => x.Name, x => x.GetValue(enumeration));
            sb.AppendLine("Enum." + typeof(T).Name + " = " + System.Web.Helpers.Json.Encode(enums) + " ;");
            sb.AppendLine("</script>");
            return new HtmlString(sb.ToString());
        }

        public static IHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string altText,
            string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes = null)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            var link = helper.ActionLink("[replaceme]", actionName, routeValues, ajaxOptions).ToHtmlString();
            return MvcHtmlString.Create(link.Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing)));
        }
        public static MvcHtmlString IconActionLink(this AjaxHelper helper, string icon, string text, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var builder = new TagBuilder("i");
            builder.MergeAttribute("class", icon);
            var link = helper.ActionLink("[replaceme] " + text, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes).ToHtmlString();
            return new MvcHtmlString(link.Replace("[replaceme]", builder.ToString()));
        }

        public static MvcHtmlString IconActionLink(this HtmlHelper helper, string icon, string text, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            var builder = new TagBuilder("i");
            builder.MergeAttribute("class", icon);
            var link = helper.ActionLink("[replaceme] " + text, actionName, controllerName, routeValues, htmlAttributes).ToHtmlString();
            return new MvcHtmlString(link.Replace("[replaceme]", builder.ToString()));
        }
    }
}