using System;

namespace Blossom
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
            var type = value.GetType();

            // Get fieldinfo for this type
            var fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return (attribs != null && attribs.Length > 0) ? attribs[0].Value : null;
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