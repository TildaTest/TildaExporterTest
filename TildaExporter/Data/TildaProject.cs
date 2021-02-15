using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TildaExporter.Data
{
    public class TildaProject
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("descr")]
        public string Descr { get; set; }

        [JsonProperty("customdomain")]
        public string Customdomain { get; set; }

        [JsonProperty("export_csspath")]
        public string ExportCssPath { get; set; }

        [JsonProperty("export_jspath")]
        public string ExportJsPath { get; set; }

        [JsonProperty("export_imgpath")]
        public string ExportImgPath { get; set; }

        [JsonProperty("indexpageid")]
        public string EndexPageId { get; set; }

        [JsonProperty("css")]
        public List<FileExportData> CssFiles { get; set; }

        [JsonProperty("js")]
        public List<FileExportData> JsFiles { get; set; }

        [JsonProperty("images")]
        public List<FileExportData> Images { get; set; }

        [JsonProperty("htaccess")]
        public string Htaccess { get; set; }
    }
}
