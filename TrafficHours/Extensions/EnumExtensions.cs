using System;
using System.ComponentModel;
using System.Reflection;

namespace Utilities
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            //throw new ArgumentException("Not found.", "description");
            // or 
            return default(T);
        }

        public static T GetValueFromValueName<T>(string valueName)
        {
            var type = typeof(T);
            T result = (T)Enum.Parse(typeof(T), valueName);

            return result;
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static bool IsValueExist<T>(string strName)
        {
            if (strName.Length > 0)
            {
                foreach (var typeName in Enum.GetNames(typeof(T)))
                {
                    if (strName == typeName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
