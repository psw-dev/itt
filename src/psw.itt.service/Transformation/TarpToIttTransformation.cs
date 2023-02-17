// using System;
// using System.Text.RegularExpressions;
// using PSW.ITT.Service.Command;
// using PSW.Lib.Logs;
// using System.Collections.Generic;
// using System.Linq;


// namespace PSW.ITT.Service.Transformation
// {
//     public class TarpToIttTransformation
//     {
//         private  
        
//         public TarpToIttTransformation()
//         {
            
//         }

//         public PSW.ITT.Service.DTO.GetDocumentRequirementRequest transform(PSW.ITT.Service.TarpDTO.GetDocumentRequirementRequest pRequestDTO , UnitOfWork ittUoW, SHRDUnitOfWork sharedUow)        
//         {
//             PSW.ITT.Service.DTO.GetDocumentRequirementRequest  requestDTO=new PSW.ITT.Service.DTO.GetDocumentRequirementRequest();
//             requestDTO.ProductCodeID=null;

//         // [JsonPropertyName("hsCode")]
//         // public string HsCode { get; set; }

//         // [JsonPropertyName("agencyId")]
//         // public string AgencyId { get; set; }

//         // [JsonPropertyName("documentTypeCode")]
//         // public string documentTypeCode { get; set; }

//         // [JsonPropertyName("factorLabelValuePairList")]
//         // public Dictionary<string, FactorData> FactorCodeValuePair { get; set; }

//         // [JsonPropertyName("tradeTranTypeId")]
//         // public int TradeTranTypeID { get; set; }

//             return requestDTO;
//         } 

//     }
// }