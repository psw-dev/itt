using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class BulkAgencyAssociationRequestDTO
    {
        [JsonPropertyName("isAdd")]
        public bool isAdd { get; set; }

        [JsonPropertyName("productCodes")]
        public List<long> ProductCodes { get; set; }
        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; }
    }
}
