using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace PITB.CMS_Common.Helper.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
        public static string GetDescription(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DescriptionAttribute>()
                            .Description;
        }
        public static string GetPluralName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<PluralNameAttribute>()
                            .PluralName;
        }
    }
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = false,Inherited = false)]
    public class PluralNameAttribute:Attribute
    {
        public string PluralName { get; set; }
        public PluralNameAttribute(string plural)
        {
            PluralName = plural;
        }
    }
}