using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace PSW.ITT.Data.Entities
{
    public class ViewRegulatedHsCode
    {
        [JsonPropertyName("hsCode")]
        public string HsCode { get; set; }

        [JsonPropertyName("agencyId")]
        public int AgencyID { get; set; }

        [JsonPropertyName("hscodeDetails")]
        public List<HscodeDetails> HsCodeDetailsList { get; set; }
    }

    public class HscodeDetails
    {
        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("itemDescription")]
        public string ItemDescription { get; set; }

        [JsonPropertyName("itemDescriptionExt")]
        public string ItemDescriptionExt { get; set; }

        [JsonPropertyName("technicalName")]
        public string TechnicalName { get; set; }
    }


}
