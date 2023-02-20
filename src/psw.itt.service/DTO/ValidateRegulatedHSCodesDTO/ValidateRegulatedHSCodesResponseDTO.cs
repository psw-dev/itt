using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace PSW.ITT.Service.DTO
{
    public class ValidateRegulatedHSCodesResponseDTO
    {
        [JsonPropertyName("hsCodes")]
        public List<string> HSCodes { set; get; }
        
    }
}