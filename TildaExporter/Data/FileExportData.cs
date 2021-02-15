using Newtonsoft.Json;

namespace TildaExporter.Data
{
    public class FileExportData
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }
}
