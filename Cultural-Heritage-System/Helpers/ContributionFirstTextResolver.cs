using AutoMapper;
using Cultural_Heritage_System.Models;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace Cultural_Heritage_System.Helpers
{

    public class ContributionFirstTextResolver
    : IValueResolver<Contribution, ContributionSearchResponse, string>
    {
        public string Resolve(Contribution src, ContributionSearchResponse dest, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(src.Content)) return string.Empty;

            // Thử parse dạng Quill Delta: { "ops": [ { insert: "..." }, { insert: { image: ... } }, ... ] }
            try
            {
                using var doc = JsonDocument.Parse(src.Content);
                if (doc.RootElement.TryGetProperty("ops", out var ops) && ops.ValueKind == JsonValueKind.Array)
                {
                    var text = "";
                    foreach (var op in ops.EnumerateArray())
                    {
                        if (op.TryGetProperty("insert", out var ins))
                        {
                            if (ins.ValueKind == JsonValueKind.String)
                            {
                                text += ins.GetString();
                            }
                            // ảnh/video thì bỏ qua
                        }
                    }
                    return TrimAndClean(text);
                }
            }
            catch
            {
                // ignore, fallback phía dưới
            }

            // Fallback: nếu content là HTML hoặc text thuần → loại bỏ tag rồi cắt gọn
            return TrimAndClean(src.Content);
        }

        private static string TrimAndClean(string? input, int max = 200)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // bỏ tag HTML nếu có
            var noHtml = Regex.Replace(input, "<.*?>", string.Empty);
            // gom khoảng trắng
            noHtml = Regex.Replace(noHtml, @"\s+", " ").Trim();

            if (noHtml.Length <= max) return noHtml;
            return noHtml.Substring(0, max) + "…";
        }
    }
}

