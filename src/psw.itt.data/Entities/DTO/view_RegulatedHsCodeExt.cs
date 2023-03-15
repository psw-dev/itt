using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace PSW.ITT.Data.Entities
{
    public class ViewRegulatedHsCodeExt
    {
        [JsonPropertyName("hsCodeExt")]
        public string HsCodeExt { get; set; }

        [JsonPropertyName("hsCodeDescription")]
        public string HsCodeDescription { get; set; }

        [JsonPropertyName("hsCodeDescriptionExt")]
        public string HsCodeDescriptionExt { get; set; }

        [JsonPropertyName("agencyId")]
        public int AgencyID { get; set; }

    }
}
