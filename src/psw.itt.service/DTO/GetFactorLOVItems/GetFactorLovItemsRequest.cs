using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetFactorLovItemsRequest
    {
        [JsonPropertyName("agencyId")]
        public int AgencyId { set; get; }

        [JsonPropertyName("documentTypeCode")]
        public string DocumentTypeCode { set; get; }

        [JsonPropertyName("tradeTranTypeId")]
        public int TradeTranTypeID { set; get; }

        [JsonPropertyName("factorList")]
        public List<FactorInfo> FactorList { get; set; }

        [JsonPropertyName("hsCodeExt")]
        public string HSCodeExt { set; get; }
    }

    public class FactorInfo
    {
        [JsonPropertyName("factorId")]
        public int FactorId { set; get; }

        [JsonPropertyName("LOVTable")]
        public string LOVTableName { get; set; }

        [JsonPropertyName("LOVColumn")]
        public string LOVColumnName { get; set; }
    }
}