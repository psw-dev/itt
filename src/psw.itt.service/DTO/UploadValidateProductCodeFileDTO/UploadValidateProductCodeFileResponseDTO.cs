using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadValidateProductCodeFileResponseDTO
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
        
        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; } 
    }

}
