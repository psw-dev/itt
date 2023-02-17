using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetListOfAgencyAgainstHsCodeRequest
    {
        [JsonPropertyName("hsCode")]
        public string HsCode { get; set; }

        [JsonPropertyName("tradeTranTypeId")]
        public int tradeTranTypeId { get; set; }

        [JsonPropertyName("documentCode")]
        public string DocumentCode { get; set; }
    }
}