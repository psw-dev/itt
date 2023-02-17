// using System;
// using System.Text.RegularExpressions;
// using PSW.ITT.Service.Command;
// using PSW.Lib.Logs;
// using System.Collections.Generic;
// using System.Linq;


// namespace PSW.ITT.Service.Transformation
// {
//     public class ITT2TarpTransformation
//     {
//         private  
        
//         public ITT2TarpTransformation()
//         {
            
//         }

//         public PSW.ITT.Service.TarpDTO.GetDocumentRequirementResponse transform(PSW.ITT.Service.DTO.GetDocumentRequirementResponse pResponseDTO , UnitOfWork ittUoW, SHRDUnitOfWork sharedUow)        
//         {
//             PSW.ITT.Service.DTarpDTO.GetDocumentRequirementResponse  responseDTO=new PSW.ITT.Service.DTO.GetDocumentRequirementResponse();
//             responseDTO.ProductCodeID=null;

//             //{"hsCode":"0100.1001","productCode":"0100.1001.1079","purpose":"Processing","technicalName":"","bannedCountries":"Afghanistan, Antigua and Barbuda, Anguilla, Albania, Armenia","ipRequired":"yes","ipMandatoryDocumentryRequirements":"Certificate of storage facilities ","ipOptionalDocumentryRequirements":"","ipFees":"5000","ipValidity":"12","ipAmendmentFees":"2000","ipExtensionAllowed":"yes","ipExtensionPeriod":"12","ipExtensionFees":"200","ipQuantityAllowed":"300","ipQuantityAllowedUnit":"kg","isSeedEnlistmentRequired":"yes","roRequired":"yes","roMandatoryDocumentryRequirements":"Certificate of Origin","roOptionalDocumentryRequirements":"","roFees":"5000","ipCertificateFormNumber":"","roCertificateFormNumber":"","destructionOrDeportationOrder":""}

//             if (RequestDTO.AgencyId == "2")
//                 {
//                     DocumentIsRequired = mongoDBRecordFetcher.CheckIfLPCORequired(mongoDoc, docType.DocumentClassificationCode, out IsParenCodeValid);
//                 }
//                 else if (RequestDTO.AgencyId == "3")
//                 {
//                     DocumentIsRequired = mongoDBRecordFetcher.CheckIfLPCORequiredAQD(mongoDoc, docType.DocumentClassificationCode, out IsParenCodeValid);
//                 }
//                 else if (RequestDTO.AgencyId == "4")
//                 {
//                     DocumentIsRequired = mongoDBRecordFetcher.CheckIfLPCORequiredFSCRD(mongoDoc, docType.DocumentClassificationCode, out IsParenCodeValid);
//                 }
//                 else if (RequestDTO.AgencyId == "5")
//                 {
//                     DocumentIsRequired = mongoDBRecordFetcher.CheckIfLPCORequiredPSQCA(mongoDoc, docType.DocumentClassificationCode, out IsParenCodeValid);
//                 }
//                 else if (RequestDTO.AgencyId == "10")
//                 {
//                     DocumentIsRequired = mongoDBRecordFetcher.CheckIfLPCORequiredMFD(mongoDoc, docType.DocumentClassificationCode, out IsParenCodeValid);
//                 }


//             return responseDTO;
//         } 

//     }
// }