using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class DownloadJSONExcelResponseDTO
    {
        [JsonPropertyName("gridColumns")]
        public List<GridColumns> GridColumns { get; set; }

        [JsonPropertyName("data")]
        public List<dynamic> Data { get; set; }

    }
}
