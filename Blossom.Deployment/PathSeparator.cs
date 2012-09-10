using System;
using System.Reflection;

namespace Blossom.Deployment
{
    internal class StringValueAttribute : Attribute
    {
        public string Value { get; set; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }

    public static class PathSeparatorExtension
    {
        public static string Value(this PathSeparator value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].Value : null;
        }
    }

    public enum PathSeparator
    {
        [StringValue("/")]
        ForwardSlash,

        [StringValue(@"\")]
        Backslash
    }
}