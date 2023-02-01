using System.Text.Json.Serialization;

namespace PSW.OGA.Service.DTO
{
    public class UploadProgressResponseDTO
    {

        [JsonPropertyName("totalRecordsCount")]
        public long TotalRecordsCount { get; set; }

        [JsonPropertyName("processedRecordsCount")]
        public long ProcessedRecordsCount { get; set; }

        [JsonPropertyName("disputedRecordsCount")]
        public long DisputedRecordsCount { get; set; }

        [JsonPropertyName("disputedRecordsData")]
        public string DisputedRecordsData { get; set; }

        [JsonPropertyName("statusId")]
        public int StatusId { get; set; }
    }
}
