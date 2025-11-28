using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Reflection;

namespace Cultural_Heritage_System.Helpers
{
    public static class ExcelExporter
    {
        public static byte[] ExportToExcel<T>(
            IEnumerable<T> items,
            Dictionary<string, string>? headers = null) // <-- optional
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Export");

            // =======================================
            // Lấy danh sách property của model T
            // =======================================
            var props = typeof(T).GetProperties();

            // Nếu không truyền headers → tạo header mặc định
            headers ??= props.ToDictionary(
                p => p.Name,
                p => p.Name // dùng chính tên property
            );

            int row = 1;
            int col = 1;

            // =======================================
            // 1) GHI HEADER (tiếng Việt nếu có)
            // =======================================
            foreach (var header in headers)
            {
                ws.Cells[row, col].Value = header.Value;
                ws.Cells[row, col].Style.Font.Bold = true;
                ws.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                ws.Column(col).Width = 25;
                col++;
            }

            // =======================================
            // 2) GHI DATA
            // =======================================
            row = 2;

            foreach (var item in items)
            {
                col = 1;

                foreach (var header in headers)
                {
                    var value = GetValue(item, header.Key);
                    ws.Cells[row, col].Value = value ?? "";
                    col++;
                }

                row++;
            }

            return package.GetAsByteArray();
        }

        // ================================================
        // Xử lý nested property: "Location.Province"
        // ================================================
        private static object? GetValue(object obj, string propPath)
        {
            foreach (var prop in propPath.Split('.'))
            {
                if (obj == null) return null;

                var type = obj.GetType();
                var p = type.GetProperty(prop);

                if (p == null) return null;

                obj = p.GetValue(obj);
            }

            if (obj == null)
                return null;

            // ===============================
            // FIX DATE → STRING
            // ===============================
            if (obj is DateTime dt)
                return dt.ToString("yyyy-MM-dd HH:mm:ss");         

            return obj;
        }

    }
}
