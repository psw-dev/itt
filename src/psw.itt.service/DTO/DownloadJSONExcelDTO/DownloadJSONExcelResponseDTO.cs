using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class DownloadJSONExcelResponseDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("data")]
        public dynamic Data { get; set; }

    }
}
