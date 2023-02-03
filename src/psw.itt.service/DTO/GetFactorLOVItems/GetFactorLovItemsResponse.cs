using PSW.ITT.Data.DTO;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetFactorLovItemsResponse
    {
        [JsonPropertyName("factorLOVItemsList")]
        public List<FactorLOVItemsData> FactorLOVItemsList { get; set; }
    }

    public class FactorLOVItemsData
    {
        [JsonPropertyName("factorID")]
        public int FactorID { get; set; }

        [JsonPropertyName("factorLabel")]
        public string FactorLabel { get; set; }

        [JsonPropertyName("factorLOVItems")]
        public List<FactorLOVItems> FactorLOVItems { get; set; }
    }
}