using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class FetchRegulatoryDataAttributeResponseDTO
    {
        [JsonPropertyName("nameLong")]
        public string NameLong { get; set; }

        [JsonPropertyName("nameShort")]
        public string NameShort { get; set; }

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("fieldControlTypeID")]
        public short FieldControlTypeID { get; set; }

        [JsonPropertyName("hint")]
        public string Hint { get; set; }

        [JsonPropertyName("maxLength")]
        public short MaxLength { get; set; }

        [JsonPropertyName("tableName")]
        public string TableName { get; set; }

        [JsonPropertyName("columnName")]
        public string ColumnName { get; set; }
        [JsonPropertyName("isEditable")]
        public bool IsEditable { get; set; }
        [JsonPropertyName("serviceName")]
        public string ServiceName { get; set; }
    }
}
