using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class DeleteRegulatoryDataRequestDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("immediately")]
        public bool Immediately { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

    }
}
