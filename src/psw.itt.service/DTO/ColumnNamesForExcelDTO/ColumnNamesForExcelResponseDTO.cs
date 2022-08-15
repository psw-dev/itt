using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class ColumnNamesForExcelResponseDTO
    {
        [JsonPropertyName("columnName")]
        public string ColumnName { get; set; }

        [JsonPropertyName("columnIndex")]
        public short Index { get; set; }
    }
}
