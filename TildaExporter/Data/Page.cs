using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TildaExporter.Data
{
    class PageShort
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("projectid")]
        public string ProjectId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("descr")]
        public string Descr { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("featureimg")]
        public string FeatureImg { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }

        [JsonProperty("published")]
        public string Published { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }
    }

    class PageFull : PageShort
    {
        [JsonProperty("images")]
        public List<FileExportData> Images { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

    }
}
