using System.Collections.Generic;
using System.Text.Json.Serialization;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Service.DTO
{
    public class FetchLOVDataResponseDTO
    {
        [JsonPropertyName("LOVItems")]
        public List<LOVItem> LOVItems { get; set; }

        [JsonPropertyName("lovColumnName")]
        public string LOVColumnName { get; set; }

        [JsonPropertyName("lovTableName")]
        public string LOVTableName { get; set; }

    }
}
