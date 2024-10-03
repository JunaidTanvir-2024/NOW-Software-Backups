using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace TalkHome.Extensions
{
    /// <summary>
    /// Extension class for Enum types
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the `Display name` attribute
        /// </summary>
        /// <param name="enumType">The Enum type</param>
        /// <returns>The display name</returns>
        public static string GetDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }

        public static class EnumHelper<T>
        {
            public static T GetValueFromName(string name)
            {
                var type = typeof(T);
                if (!type.IsEnum) throw new InvalidOperationException();

                foreach (var field in type.GetFields())
                {
                    var attribute = Attribute.GetCustomAttribute(field,
                        typeof(DisplayAttribute)) as DisplayAttribute;
                    if (attribute != null)
                    {
                        if (attribute.Name == name)
                        {
                            return (T)field.GetValue(null);
                        }
                    }
                    else
                    {
                        if (field.Name == name)
                            return (T)field.GetValue(null);
                    }
                }

                throw new ArgumentOutOfRangeException("name");
            }
        }
    }
}