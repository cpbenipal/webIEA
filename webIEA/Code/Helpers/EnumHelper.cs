using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

namespace Flexpage.Code.Helpers
{
    /// <summary>
    /// Class for working with Enums
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Returns description of the enum.
        /// </summary>
        /// <param name="en">Enum</param>
        /// <returns>Description of the enum</returns>
        /// <remarks>Description for the enum should be specified in DescriptionAttribute attribute, 
        /// see example</remarks>
        /// <example>
        ///     public enum MyEnum{ 
        ///        [DescriptionAttribute("Enum item description")]
        ///        EnumItem };
        /// </example>
        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if(memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
                if(attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }

        public static string GetDisplay(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if(memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(
                    typeof(DisplayAttribute), false);
                if(attrs != null && attrs.Length > 0)
                    return ((DisplayAttribute)attrs[0]).Name;
            }
            return en.ToString();
        }

        public static MvcHtmlString CheckboxListForEnum<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression,
             IDictionary<string, object> htmlAttributes = null) where TProperty : struct, IConvertible
        {
            if(!typeof(TProperty).IsEnum)
                throw new ArgumentException("TProperty must be an enumerated type");

            TProperty value = expression.Compile()((TModel)html.ViewContext.ViewData.Model);


            var enumValue = (Enum)Enum.Parse(typeof(TProperty), value.ToString());


            var itens = Enum
                .GetValues(typeof(TProperty)).Cast<Enum>()
                .Select(c => new SelectListItem
                {
                    Text = c.GetDescription(),
                    Value = c.ToString(),
                    Selected = null != enumValue && enumValue.HasFlag(c)
                });

            var name = ExpressionHelper.GetExpressionText(expression);

            var sb = new StringBuilder();
            var ul = new TagBuilder("ul");

            ul.MergeAttributes(htmlAttributes);

            foreach(var item in itens)
            {
                var id = string.Format("{0}_{1}", name, item.Value);

                var li = new TagBuilder("li");

                var checkBox = new TagBuilder("input");
                checkBox.Attributes.Add("id", id);
                checkBox.Attributes.Add("value", item.Value);
                checkBox.Attributes.Add("name", name);
                checkBox.Attributes.Add("type", "checkbox");
                if(item.Selected)
                    checkBox.Attributes.Add("checked", "checked");

                var label = new TagBuilder("label");
                label.Attributes.Add("for", id);

                label.SetInnerText(item.Text);

                li.InnerHtml = checkBox.ToString(TagRenderMode.SelfClosing) + "\r\n" +
                               label.ToString(TagRenderMode.Normal);

                sb.AppendLine(li.ToString(TagRenderMode.Normal));
            }

            ul.InnerHtml = sb.ToString();

            return new MvcHtmlString(ul.ToString(TagRenderMode.Normal));
        }
    }

    public static class Enum<T>
    {
        /// <summary>
        /// Parses string value as enum of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="en"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Parses string value as enum of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="en"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out T x)
        {
            try
            {
                x = (T)Enum.Parse(typeof(T), value);
                return true;
            }
            catch
            {
                x = (T)Enum.Parse(typeof(T), Enum.GetNames(typeof(T))[0]);
                return false;
            }
        }

        /// <summary>
        /// Return all values of the enum of type T
        /// </summary>
        /// <returns>Enums</returns>
        public static IList<T> GetValues()
        {
            IList<T> list = new List<T>();
            foreach(object value in Enum.GetValues(typeof(T)))
            {
                list.Add((T)value);
            }
            return list;
        }  
    }
}
