using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadConfigrationFileResponseDTO
    {

        [JsonPropertyName("disputedRecordCount")]
        public long DisputedRecordCount { get; set; }
        
        [JsonPropertyName("duplicateRecordCount")]
        public long DuplicateRecordCount { get; set; }

        [JsonPropertyName("totalRecordCount")]
        public long TotalRecordCount { get; set; }

        [JsonPropertyName("processedRecordsCount")]
        public long ProcessedRecordsCount { get; set; }

        [JsonPropertyName("statusID")]
        public short StatusID { get; set; }

        [JsonPropertyName("gridColumns")]
        public List<GridColumns> GridColumns { get; set; }

        [JsonPropertyName("data")]
        public List<dynamic> Data { get; set; }
    }

     public class GridColumns
    {

        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("editor")]
        public string Editor { get; set; }

        [JsonPropertyName("width")]
        public string Width { get; set; }
    }
}
