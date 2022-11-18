using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UpdateSingleRegulationRequestDTO
    {
        [JsonPropertyName("data")]
        public dynamic Data { get; set; }

        [JsonPropertyName("id")]
        public long ID { get; set; }
    }
}
