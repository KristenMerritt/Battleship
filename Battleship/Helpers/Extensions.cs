using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Battleship.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// https://stackoverflow.com/a/36316189
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headerName"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static T GetHeaderValueAs<T>(this string headerName, HttpContext ctx)
        {
            StringValues values;

            if (ctx?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!rawValues.IsNullOrEmpty())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }

        /// <summary>
        /// https://stackoverflow.com/a/36316189
        /// </summary>
        /// <param name="csvList"></param>
        /// <param name="nullOrWhitespaceInputReturnsNull"></param>
        /// <returns></returns>
        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return String.IsNullOrEmpty(s);
        }

        public static string PropsToString(this object obj)
        {
            var sb = new StringBuilder();
            var props = obj.GetType().GetProperties();
            sb.AppendLine(obj.GetType().Name + " Properties:");
            foreach (var prop in props)
            {
                sb.Append("  " + prop.Name + " = ");
                var val = prop.GetValue(obj);
                sb.AppendLine(val == null ? "NULL" : val.ToString());
            }
            return sb.ToString();
        }
    }
}
