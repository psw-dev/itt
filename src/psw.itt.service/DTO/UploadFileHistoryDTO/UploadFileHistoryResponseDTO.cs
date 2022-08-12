using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadFileHistoryResponseDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("agencyId")]
        public int AgencyID { get; set; }

        [JsonPropertyName("ownerDocumentTypeCode")]
        public string OwnerDocumentTypeCode { get; set; }

        [JsonPropertyName("ownerDocumentTypeName")]
        public string OwnerDocumentTypeName { get; set; }

        [JsonPropertyName("attachedFileID")]
        public string AttachedFileID { get; set; }

        [JsonPropertyName("attachedFileName")]
        public string AttachedFileName { get; set; }

        [JsonPropertyName("totalRecordsCount")]
        public long TotalRecordsCount { get; set; }

        [JsonPropertyName("processedRecordsCount")]
        public long ProcessedRecordsCount { get; set; }

        [JsonPropertyName("duplicateRecordsCount")]
        public long DuplicateRecordsCount { get; set; }

        [JsonPropertyName("disputedRecordsCount")]
        public long DisputedRecordsCount { get; set; }

        [JsonPropertyName("disputedRecordsData")]
        public string DisputedRecordsData { get; set; }

        [JsonPropertyName("statusId")]
        public int StatusId { get; set; }

        [JsonPropertyName("statusName")]
        public string StatusName { get; set; }

        [JsonPropertyName("createdOn")]
        public string CreatedOn { get; set; }

        [JsonPropertyName("updatedOn")]
        public string UpdatedOn { get; set; }

        [JsonPropertyName("isLast")]
        public bool IsLast { get; set; }

        [JsonPropertyName("uploadedBy")]
        public string UploadedBy { get; set; }
    }
}
