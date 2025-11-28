using System.Text;
using System.Collections;
using System.Reflection;

namespace Cultural_Heritage_System.Helpers
{
    public static class CsvExporter
    {
        public static byte[] ExportToCsv<T>(
            IEnumerable<T> items,
            Dictionary<string, string>? headers = null)
        {
            var flatList = items
                .Select(item => FlattenObjectClean(item))
                .ToList();

            var allKeys = flatList
                .SelectMany(d => d.Keys)
                .Distinct()
                .ToList();

            headers ??= allKeys.ToDictionary(k => k, k => k);

            var sb = new StringBuilder();

            sb.AppendLine(string.Join(",", headers.Values.Select(Escape)));

            foreach (var row in flatList)
            {
                var line = headers.Keys
                    .Select(key => Escape(row.ContainsKey(key) ? row[key] ?? "" : ""))
                    .ToList();

                sb.AppendLine(string.Join(",", line));
            }

            var bom = Encoding.UTF8.GetPreamble();
            var body = Encoding.UTF8.GetBytes(sb.ToString());

            var result = new byte[bom.Length + body.Length];
            Buffer.BlockCopy(bom, 0, result, 0, bom.Length);
            Buffer.BlockCopy(body, 0, result, bom.Length, body.Length);

            return result;
        }

        private static string Escape(string value)
        {
            value ??= "";

            value = value
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("\"", "\"\"");

            return $"\"{value}\"";
        }

        /// <summary>
        /// Clean flatten logic – list object → readable string, 
        /// no nested Tag.0.Name keys.
        /// </summary>
        private static Dictionary<string, string?> FlattenObjectClean(object obj, string prefix = "")
        {
            var result = new Dictionary<string, string?>();

            if (obj == null)
                return result;

            var type = obj.GetType();

            if (obj is string || type.IsValueType)
            {
                result[prefix] = obj.ToString();
                return result;
            }

            if (obj is IEnumerable list && !(obj is string))
            {
                var items = new List<string>();

                foreach (var item in list)
                {
                    if (item == null) continue;

                    var t = item.GetType();
                    var props = t.GetProperties();

                    if (props.Any())
                    {
                        var readable = string.Join(" | ",
                            props.Select(p => $"{p.Name}: {p.GetValue(item)}"));

                        items.Add(readable);
                    }
                    else
                    {
                        items.Add(item.ToString());
                    }
                }

                result[prefix] = string.Join("; ", items);
                return result;
            }

            foreach (var prop in type.GetProperties())
            {
                var value = prop.GetValue(obj);
                var key = string.IsNullOrEmpty(prefix)
                    ? prop.Name
                    : $"{prefix}.{prop.Name}";

                if (value == null)
                {
                    result[key] = null;
                }
                else if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    result[key] = value.ToString();
                }
                else
                {
                    var nested = FlattenObjectClean(value, key);
                    foreach (var kv in nested)
                        result[kv.Key] = kv.Value;
                }
            }

            return result;
        }
    }
}
