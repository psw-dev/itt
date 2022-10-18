using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class DeleteRegulatoryDataRequestDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

    }
}
