using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class CloseProductCodeRequestDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

    }
}
