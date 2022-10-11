using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class FetchRegulatoryDataAttributeResponseDTO
    {
        [JsonPropertyName("nameLong")]
        public string NameLong { get; set; }

        [JsonPropertyName("nameLong")]
        public string NameShort { get; set; }

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("fieldControlTypeID")]
        public short FieldControlTypeID { get; set; }

        [JsonPropertyName("hint")]
        public string Hint { get; set; }

        [JsonPropertyName("maxLength")]
        public short MaxLength { get; set; }

    }
}
