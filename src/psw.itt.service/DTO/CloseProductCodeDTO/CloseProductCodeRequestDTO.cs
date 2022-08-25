using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class CloseProductCodeRequestDTO
    {
        [JsonPropertyName("hSCode")]
        public string HSCode { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

    }
}
