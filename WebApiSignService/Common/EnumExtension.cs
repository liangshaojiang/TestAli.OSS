using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApiSignService.Models;

namespace WebApiSignService.Common
{
    public static class EnumExtension
    {

        public static string GetEnumText(ExceptionStatus en)
        {
            if (en == null)
                return string.Empty;
            var field = en.GetType().GetField(en.ToString());
            string enstring = en.ToString();
            var attrs = field.GetCustomAttributes(typeof(TextAttribute), false);
            if (attrs.Length == 1)
            {
                enstring = ((TextAttribute)attrs[0]).value;
            }
            return enstring;
        }
    }

    public class TextAttribute : Attribute
    {
        public TextAttribute(string value)
        {
            this.value = value;
        }

        public string value { get; set; }
    }
}