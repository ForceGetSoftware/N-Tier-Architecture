using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace N_Tier.Application.Helpers
{
    public static class ExportExcelViaCSV
    {
        public static FileContentResult GenerateCsv<T>(List<T> data, Dictionary<string, string> keyValuePairs, string fileName)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8);

            var properties = typeof(T).GetProperties().Where(prop => keyValuePairs.ContainsKey(prop.Name)).ToList();
            var headers = properties.Select(prop => keyValuePairs[prop.Name]);
            writer.WriteLine(string.Join(",", headers));

            foreach (var item in data)
            {
                var values = properties.Select(prop =>
                {
                    var value = prop.GetValue(item)?.ToString() ?? string.Empty;
                    return $"\"{value}\"";
                });
                writer.WriteLine(string.Join(",", values));
            }

            writer.Flush();
            var content = stream.ToArray();

            return new FileContentResult(content, "text/csv") { FileDownloadName = fileName };
        }
    }
}
