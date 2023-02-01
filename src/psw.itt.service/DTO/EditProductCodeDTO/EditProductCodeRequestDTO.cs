using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class EditProductCodeRequestDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("effectiveThruDt")]
        public DateTime EffectiveThruDt { get; set; }

    }
}
