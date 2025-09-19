using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace Cultural_Heritage_System.Helpers
{
    public static class DeltaHelper
    {
        public static string GeneratePreviewDelta(string contentDeltaJson, double ratio = 0.33, int minLength = 100)
        {
            if (string.IsNullOrEmpty(contentDeltaJson))
                return contentDeltaJson;

            var root = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentDeltaJson);
            if (root == null || !root.ContainsKey("ops"))
                return contentDeltaJson;

            var ops = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(root["ops"].ToString());
            if (ops == null || ops.Count == 0)
                return contentDeltaJson;

            int totalLength = ops.Sum(op => GetOpLength(op));
            int limit = Math.Min(totalLength, Math.Max((int)(totalLength * ratio), minLength));

            var previewOps = new List<Dictionary<string, object>>();
            int currentCount = 0;

            foreach (var op in ops)
            {
                if (!op.ContainsKey("insert")) continue;

                var insert = op["insert"];

                if (insert is string text)
                {
                    int len = text.Length;
                    if (currentCount + len <= limit)
                    {
                        previewOps.Add(op); // copy nguyên op
                        currentCount += len;
                    }
                    else
                    {
                        int take = limit - currentCount;
                        if (take > 0)
                        {
                            var newOp = new Dictionary<string, object>(op);
                            newOp["insert"] = text.Substring(0, take) + "..."; 
                            previewOps.Add(newOp);
                        }
                        break; 
                    }
                }
                else
                {
                    // Non-text (ảnh, video, embed...) → giữ nguyên luôn
                    previewOps.Add(op);
                }
            }

            return JsonConvert.SerializeObject(new { ops = previewOps });
        }

        private static int GetOpLength(Dictionary<string, object> op)
        {
            if (op.ContainsKey("insert") && op["insert"] is string text)
                return text.Length;

            return 0; // ảnh, video... không tính độ dài
        }
    }


}

