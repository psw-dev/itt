using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace PSW.ITT.Service.DTO
{
    public class AQDECFeeCalculateResponseDTO
    {
        [JsonPropertyName("amount")]
        public string Amount { get; set; }

    }
}