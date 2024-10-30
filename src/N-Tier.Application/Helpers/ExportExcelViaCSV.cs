using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace N_Tier.Application.Helpers
{
    public static class ExportExcelViaCSV
    {
        public static FileContentResult GenerateCsv<T>(List<T> data, List<string> headers, string fileName)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            var properties = typeof(T).GetProperties();

            writer.WriteLine(string.Join(",", headers));

            foreach (var item in data)
            {
                var values = new List<string>();
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item)?.ToString() ?? string.Empty;
                    values.Add(value);
                }
                writer.WriteLine(string.Join(",", values));
            }

            writer.Flush();
            var content = stream.ToArray();

            return new FileContentResult(content, "text/csv") { FileDownloadName = fileName };
        }
    }
}
