using System.ComponentModel;
using System.Reflection;

namespace Hamburglar
{
    public enum Hosts
    {
        [DescriptionAttribute("localhost:23019")]
        LocalDebug,

        [DescriptionAttribute("weenus.hamburglar.com")]
        Local,

        [DescriptionAttribute("hamburglar.weenussoft.com")]
        Prod
    }
    public static class _HostsExtensions
    {
        public static string GetDescription(this Hosts value)
        {
            var f = value.GetType().GetField(value.ToString());
            if (f == null)
            {
                return string.Empty;
            }
            var attrs = f.GetCustomAttributes(typeof(DescriptionAttribute), true);
            return (attrs != null && attrs.Length > 0)
                        ? ((DescriptionAttribute)attrs[0]).Description
                        : string.Empty;
        }
    }
}
