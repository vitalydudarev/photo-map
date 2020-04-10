using System.Linq;

namespace Yandex.Disk.Api.Client
{
    public static class Extensions
    {
        public static string ToSnakeCase(this string str)
        {
            return string.Concat(
                str.Select(
                    (x, i) => i > 0 && char.IsUpper(x)
                        ? "_" + x
                        : x.ToString()
                )
            ).ToLower();
            
            // or
            // private const string Pattern = "(?<=.)([A-Z])";
            // private const string SnakeCaseReplacement = "_$0";
            // Regex.Replace(str, Pattern, SnakeCaseReplacement, RegexOptions.Compiled);
        }
    }
}