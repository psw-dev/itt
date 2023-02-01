using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Data.DTO
{
    public class ProductCodeValidationList
    {
        [JsonPropertyName("index")]
        public long Index { get; set; }
        [JsonPropertyName("sheetAttributeMappingID")]
        public long SheetAttributeMappingID { get; set; }

        [JsonPropertyName("validationID")]
        public int ValidationID { get; set; }

        [JsonPropertyName("validation")]
        public string Validation { get; set; }

        [JsonPropertyName("validationDescription")]
        public string ValidationDescription { get; set; }

        [JsonPropertyName("columnName")]
        public string ColumnName { get; set; }

        [JsonPropertyName("tableName")]
        public string TableName { get; set; }
    }
}
