using System.Reflection;
using System.Linq;

namespace Blossom
{
    internal static class Net45Extensions
    {
        internal static T[] GetCustomAttributes<T>(this MemberInfo method)
        {
            var attrs = method.GetCustomAttributes(typeof (T), true);
            return attrs.Cast<T>().ToArray();
        }

        internal static T GetCustomAttribute<T>(this MemberInfo method)
            where T : class
        {
            var attrs = method.GetCustomAttributes(typeof (T), true);
            return attrs.Length == 0 ? null : (T) attrs[0];
        }
    }
}
