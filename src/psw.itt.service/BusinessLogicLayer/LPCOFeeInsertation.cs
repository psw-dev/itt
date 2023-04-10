using System;
using System.Linq;
using System.Collections.Generic;
using PSW.ITT.Data.Entities;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Command;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSW.ITT.Common.Enums;
using PSW.ITT.Common.Constants;

namespace PSW.ITT.service
{
    public class LPCOFeeInsertation
    {
        private CommandRequest Command { get; set; }
        private JObject RegulationJson { get; set; }
        private List<SheetAttributeMapping> PropertyNameList { get; set; }
        private int UserRoleId  { get; set; }
        private long LpcoRegulationId { get; set; }
        private short TradeTranTypeID;
        private short AgencyID;

        public LPCOFeeInsertation(CommandRequest command, JObject jobject, List<SheetAttributeMapping> propertyNameList, int userRoleId, long lpcoRegulationId, int agencyID, short tradeTranTypeID )
        {
            this.Command = command;
            this.RegulationJson = jobject;
            this.PropertyNameList = propertyNameList;
            this.UserRoleId = userRoleId;
            this.LpcoRegulationId = lpcoRegulationId;
            this.AgencyID = (short) agencyID;
            this.TradeTranTypeID = tradeTranTypeID;
        }
        public void InsertFinancialInformation(){//JObject jobject, List<SheetAttributeMapping> PropertyNameList, int UserRoleId, long LpcoRegulationId

            LPCOFeeStructure lpcoFeeStructure = new LPCOFeeStructure();

            var calculationBasis = Command.UnitOfWork.CalculationBasisRepository.Get().ToList();
            var calculationSource = Command.UnitOfWork.CalculationSourceRepository.Get().ToList();
            var unitList = Command.SHRDUnitOfWork.Ref_UnitsRepository.Get().ToList();
            decimal n1;

            
            //for Import Permit Fees
            var feePropertyDetail = PropertyNameList.Where(x=>x.NameShort=="ipFees").FirstOrDefault();
           if (TradeTranTypeID==(short)PSW.ITT.Common.Constants.TradeTranType.IMPORT){
                if(RegulationJson.ContainsKey("ipRequired")){
                    if(getLowerValue(RegulationJson["ipRequired"]) == "yes"){
                        if(RegulationJson["ipFees"]!=null)
                        {
                            decimal? additionalAmount = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.IMPORT_PERMIT);

                            if(feePropertyDetail.NameLong.Contains("[Quantity;Unit;Price|]")){
                                
                                List<FeeDecoderResponseDTO> listFeeDecoderResponseDTO = new List<FeeDecoderResponseDTO>();
                                
                                listFeeDecoderResponseDTO = FeeDecoder(RegulationJson["ipFees"], calculationBasis, unitList, RegulationJson["ipFeeCalculationBasis"]);

                                foreach( var i in listFeeDecoderResponseDTO){

                                    mapObject(LpcoRegulationId, AgencyID, // long LpcoRegulationId, short agencyID, int? unitID,
                                    i.Unit, i.CalculationBasisValue,// int? unitID, int? calculationBasis,
                                    calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ipFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),//int? calculationSource
                                    MasterDocumentClassificationCode.IMPORT_PERMIT,//string masterDocumentClassificationCode
                                    DocumentClassificationCode.IMPORT_PERMIT,//string documentClassificationCode,
                                    i.QtyRangeTo, i.QtyRangeFrom, "PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                    i.Rate, //decimal? rate
                                    RegulationJson["ipFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["ipFeeMinimumAmount"]), out n1) ? (decimal?) n1 :null :null, // decimal? minAmount
                                    additionalAmount, //decimal? additionalAmount
                                    RegulationJson["ipFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["ipFeeAdditionalAmountOn"])) ? null : 
                                            (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ipFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                                    UserRoleId );//int UserRoleId
                                }
                            }
                            else{  
                            
                                mapObject(LpcoRegulationId, AgencyID, null,// long LpcoRegulationId, short agencyID, int? unitID,
                                calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ipFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                                calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ipFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),//int? calculationSource
                                MasterDocumentClassificationCode.IMPORT_PERMIT,//string masterDocumentClassificationCode
                                DocumentClassificationCode.IMPORT_PERMIT,//string documentClassificationCode,
                                null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                RegulationJson["ipFees"] != null ? Decimal.TryParse(getValue(RegulationJson["ipFees"]), out n1) ? (decimal?) n1 : null : null, //decimal? rate
                                RegulationJson["ipFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["ipFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null, // decimal? minAmount
                                additionalAmount, //decimal? additionalAmount
                                RegulationJson["ipFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["ipFeeAdditionalAmountOn"])) ? null : 
                                        (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ipFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                                UserRoleId );//int UserRoleId
                            }       
                        }
                        if(RegulationJson.ContainsKey("ipAmendmentFees") && RegulationJson["ipAmendmentFees"]!=null){
                            
                            decimal? additionalAmountForExtension = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.IMPORT_PERMIT_AMENDMENT);

                            mapObject(LpcoRegulationId, AgencyID, null,// long LpcoRegulationId, short agencyID, int? unitID,
                            calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ipFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                            calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ipFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? calculationSource
                            MasterDocumentClassificationCode.IMPORT_PERMIT,//string masterDocumentClassificationCode
                            DocumentClassificationCode.IMPORT_PERMIT_AMENDMENT,//string documentClassificationCode,
                            null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                            RegulationJson["ipAmendmentFees"]  != null ? Decimal.TryParse(getValue(RegulationJson["ipAmendmentFees"]), out n1) ? (decimal?)n1: null : null, //decimal? rate
                            null, // decimal? minAmount
                            RegulationJson.ContainsKey("ipExtensionAllowed") ? additionalAmountForExtension : null,//decimal? additionalAmount
                            string.IsNullOrEmpty( getLowerValue(RegulationJson["ipFeeCalculationSource"])) ? null :
                                        (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ipFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                            UserRoleId );//int UserRoleId
                        
                            
                        }
                    }
                }
            
                //for Release Order Fees
                feePropertyDetail = PropertyNameList.Where(x=>x.NameShort=="roFees").FirstOrDefault();
                
                
                if(RegulationJson.ContainsKey("roRequired")){
                    if(getLowerValue(RegulationJson["roRequired"]) == "yes"){
                       if(RegulationJson["roFees"] != null)
                        { 
                            decimal? additionalAmount = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.RELEASE_ORDER);

                            if(feePropertyDetail.NameLong.Contains("[Quantity;Unit;Price|]")){
                                
                                List<FeeDecoderResponseDTO> listFeeDecoderResponseDTO = new List<FeeDecoderResponseDTO>();

                                listFeeDecoderResponseDTO = FeeDecoder(RegulationJson["roFees"], calculationBasis, unitList, RegulationJson["roFeeCalculationBasis"]);
                                
                                foreach( var i in listFeeDecoderResponseDTO){
                                
                                    mapObject(LpcoRegulationId, AgencyID, // long LpcoRegulationId, short agencyID, int? unitID,
                                    i.Unit, i.CalculationBasisValue,// int? unitID, int? calculationBasis,
                                    calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["roFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),//int? calculationSource
                                    MasterDocumentClassificationCode.RELEASE_ORDER,//string masterDocumentClassificationCode
                                    DocumentClassificationCode.RELEASE_ORDER,//string documentClassificationCode,
                                    i.QtyRangeTo, i.QtyRangeFrom, "PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                    i.Rate, //decimal? rate
                                    RegulationJson["roFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["roFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null, // decimal? minAmount
                                    additionalAmount, //decimal? additionalAmount
                                    RegulationJson["roFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["roFeeAdditionalAmountOn"])) ? null : 
                                            (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["roFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                                    UserRoleId );//int UserRoleId

                                }
                            }
                            else{
                                mapObject(LpcoRegulationId, AgencyID, null, // long LpcoRegulationId, short agencyID, int? unitID,
                                calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["roFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                                calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["roFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),  //int? calculationSource
                                MasterDocumentClassificationCode.RELEASE_ORDER,//string masterDocumentClassificationCode
                                DocumentClassificationCode.RELEASE_ORDER,//string documentClassificationCode,
                                null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                RegulationJson["roFees"] == null ? null : Decimal.TryParse(getValue(RegulationJson["roFees"]), out n1) ? (decimal?)n1:null,//decimal? rate
                                RegulationJson["roFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["roFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null,  // decimal? minAmount
                                additionalAmount,//decimal? additionalAmount
                                RegulationJson["roFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["roFeeAdditionalAmountOn"])) ? null :
                                        (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["roFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(),//int? additionalAmountOn
                                UserRoleId );//int UserRoleId
                            
                            }
                        }
                    }
                
                //for Product Registration Fees
                feePropertyDetail = PropertyNameList.Where(x=>x.NameShort=="prdFees").FirstOrDefault();
                
                if(RegulationJson.ContainsKey("prdRequired")){
                    if(getLowerValue(RegulationJson["prdRequired"]) == "yes"){
                        
                        decimal? additionalAmount = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.PRODUCT_REGISTRATION);

                        if(feePropertyDetail.NameLong.Contains("[Quantity;Unit;Price|]")){

                            List<FeeDecoderResponseDTO> listFeeDecoderResponseDTO = new List<FeeDecoderResponseDTO>();

                            listFeeDecoderResponseDTO = FeeDecoder(RegulationJson["prdFees"], calculationBasis, unitList, RegulationJson["prdFeeCalculationBasis"]);
                            
                            foreach( var i in listFeeDecoderResponseDTO){
                                mapObject(LpcoRegulationId, AgencyID, // long LpcoRegulationId, short agencyID, int? unitID,
                                i.Unit, i.CalculationBasisValue,// int? unitID, int? calculationBasis,
                                calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prdFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),//int? calculationSource
                                MasterDocumentClassificationCode.PRODUCT_REGISTRATION,//string masterDocumentClassificationCode
                                DocumentClassificationCode.PRODUCT_REGISTRATION,//string documentClassificationCode,
                                i.QtyRangeTo, i.QtyRangeFrom, "PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                i.Rate, //decimal? rate
                                RegulationJson["prdFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["prdFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null,  // decimal? minAmount
                                additionalAmount, //decimal? additionalAmount
                                RegulationJson["prdFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["prdFeeAdditionalAmountOn"])) ? null : 
                                        (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prdFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                                UserRoleId );//int UserRoleId
                            }
                        }
                        else{
                            mapObject(LpcoRegulationId, AgencyID, null,// long LpcoRegulationId, short agencyID, int? unitID,
                            calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prdFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                            calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prdFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? calculationSource
                            MasterDocumentClassificationCode.PRODUCT_REGISTRATION,//string masterDocumentClassificationCode
                            DocumentClassificationCode.PRODUCT_REGISTRATION,//string documentClassificationCode,
                            null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                            RegulationJson["prdFees"] == null ? null : Decimal.TryParse(getValue(RegulationJson["prdFees"]), out n1) ? (decimal?)n1:null,//decimal? rate
                            RegulationJson["prdFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["prdFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null,  // decimal? minAmount
                            additionalAmount,//decimal? additionalAmount
                            RegulationJson["prdFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["prdFeeAdditionalAmountOn"])) ? null : 
                                    (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prdFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(),//int? additionalAmountOn
                            UserRoleId );//int UserRoleId
                        
                        
                        }
                    }
                }
                }
           }
           else if (TradeTranTypeID==(short)PSW.ITT.Common.Constants.TradeTranType.EXPORT){

                //for Export Certificate Fees
                feePropertyDetail = PropertyNameList.Where(x=>x.NameShort=="ecFees").FirstOrDefault();
                
                if(RegulationJson.ContainsKey("ecRequired")){
                    if(getLowerValue(RegulationJson["ecRequired"]) == "yes"){

                        decimal? additionalAmount = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.EXPORT_CERTIFICATE);

                        if(feePropertyDetail.NameLong.Contains("[Quantity;Unit;Price|]")){

                            List<FeeDecoderResponseDTO> listFeeDecoderResponseDTO = new List<FeeDecoderResponseDTO>();

                            listFeeDecoderResponseDTO = FeeDecoder(RegulationJson["ecFees"], calculationBasis, unitList, RegulationJson["ecFeeCalculationBasis"]);
                            
                            foreach( var i in listFeeDecoderResponseDTO){

                                mapObject(LpcoRegulationId, AgencyID, // long LpcoRegulationId, short agencyID, int? unitID,
                                i.Unit, i.CalculationBasisValue,// int? unitID, int? calculationBasis,
                                calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ecFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),//int? calculationSource
                                MasterDocumentClassificationCode.EXPORT_CERTIFICATE,//string masterDocumentClassificationCode
                                DocumentClassificationCode.EXPORT_CERTIFICATE,//string documentClassificationCode,
                                i.QtyRangeTo, i.QtyRangeFrom, "PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                i.Rate, //decimal? rate
                                RegulationJson["ecFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["ecFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null, // decimal? minAmount
                                additionalAmount, //decimal? additionalAmount
                                RegulationJson["ecFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["ecFeeAdditionalAmountOn"])) ? null : 
                                        (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ecFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                                UserRoleId );//int UserRoleId
                            }
                        }
                        else{
                            mapObject(LpcoRegulationId, AgencyID, null,// long LpcoRegulationId, short agencyID, int? unitID,
                            calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ecFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                            calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ecFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? calculationSource
                            MasterDocumentClassificationCode.EXPORT_CERTIFICATE,//string masterDocumentClassificationCode
                            DocumentClassificationCode.EXPORT_CERTIFICATE,//string documentClassificationCode,
                            null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                            RegulationJson["ecFees"] == null ? null : Decimal.TryParse(getValue(RegulationJson["ecFees"]), out n1) ? (decimal?)n1:null,//decimal? rate
                            RegulationJson["ecFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["ecFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null, // decimal? minAmount
                            additionalAmount,//decimal? additionalAmount
                            RegulationJson["ecFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["ecFeeAdditionalAmountOn"])) ? null : 
                                    (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ecFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(),//int? additionalAmountOn
                            UserRoleId );//int UserRoleId


                            
                        }
                    }
                }

                //for Premise Registration Fees
                feePropertyDetail = PropertyNameList.Where(x=>x.NameShort=="prmFees").FirstOrDefault();
                
                if(RegulationJson.ContainsKey("prmRequired")){
                    if(getLowerValue(RegulationJson["prmRequired"]) == "yes"){

                        decimal? additionalAmount = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.PREMISE_REGISTRATION);

                        if(feePropertyDetail.NameLong.Contains("[Quantity;Unit;Price|]")){

                            List<FeeDecoderResponseDTO> listFeeDecoderResponseDTO = new List<FeeDecoderResponseDTO>();

                            listFeeDecoderResponseDTO = FeeDecoder(RegulationJson["prmFees"], calculationBasis, unitList, RegulationJson["prmFeeCalculationBasis"]);
                            
                            foreach( var i in listFeeDecoderResponseDTO){

                                mapObject(LpcoRegulationId, AgencyID, // long LpcoRegulationId, short agencyID, int? unitID,
                                i.Unit, i.CalculationBasisValue,// int? unitID, int? calculationBasis,
                                calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prmFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),//int? calculationSource
                                MasterDocumentClassificationCode.PREMISE_REGISTRATION,//string masterDocumentClassificationCode
                                DocumentClassificationCode.PREMISE_REGISTRATION,//string documentClassificationCode,
                                i.QtyRangeTo, i.QtyRangeFrom, "PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                i.Rate, //decimal? rate
                                RegulationJson["prmFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["prmFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null, // decimal? minAmount
                                additionalAmount, //decimal? additionalAmount
                                RegulationJson["prmFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["prmFeeAdditionalAmountOn"])) ? null : 
                                        (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prmFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                                UserRoleId );//int UserRoleId
                            }
                        }
                        else{
                            mapObject(LpcoRegulationId, AgencyID, null,// long LpcoRegulationId, short agencyID, int? unitID,
                            calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prmFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                            calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prmFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? calculationSource
                            MasterDocumentClassificationCode.PREMISE_REGISTRATION,//string masterDocumentClassificationCode
                            DocumentClassificationCode.PREMISE_REGISTRATION,//string documentClassificationCode,
                            null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                            RegulationJson["prmFees"] == null ? null : Decimal.TryParse(getValue(RegulationJson["prmFees"]), out n1) ? (decimal?)n1:null,//decimal? rate
                            RegulationJson["prmFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["prmFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null, // decimal? minAmount
                            additionalAmount,//decimal? additionalAmount
                            RegulationJson["prmFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["prmFeeAdditionalAmountOn"])) ? null : 
                                    (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prmFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(),//int? additionalAmountOn
                            UserRoleId );//int UserRoleId
                        }
                        if(RegulationJson.ContainsKey("prmRenewalRequired")){

                            decimal? additionalAmountRenewal = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.PREMISE_REGISTRATION_RENEWAL);

                            mapObject(LpcoRegulationId, AgencyID, null,// long LpcoRegulationId, short agencyID, int? unitID,
                            calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prmFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                            calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prmFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? calculationSource
                            MasterDocumentClassificationCode.PREMISE_REGISTRATION,//string masterDocumentClassificationCode
                            DocumentClassificationCode.PREMISE_REGISTRATION_RENEWAL,//string documentClassificationCode,
                            null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                            RegulationJson["prmRenewalFees"] == null ? null : Decimal.TryParse(getValue(RegulationJson["prmRenewalFees"]), out n1) ? (decimal?)n1:null, //decimal? rate
                            null, // decimal? minAmount
                            additionalAmountRenewal,//decimal? additionalAmount
                            string.IsNullOrEmpty( getLowerValue(RegulationJson["prmFeeCalculationSource"])) ? null :
                                        (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["prmFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                            UserRoleId );//int UserRoleId
                         
                        }
                    }
                }

                //for Catch Certificate Fees
                feePropertyDetail = PropertyNameList.Where(x=>x.NameShort=="ccFees").FirstOrDefault();
                
                if(RegulationJson.ContainsKey("ccRequired")){
                    if(getLowerValue(RegulationJson["ccRequired"]) == "yes"){
                        decimal? additionalAmount = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.CATCH_CERTIFICATE);
                        if(feePropertyDetail.NameLong.Contains("[Quantity;Unit;Price|]")){


                            List<FeeDecoderResponseDTO> listFeeDecoderResponseDTO = new List<FeeDecoderResponseDTO>();

                            listFeeDecoderResponseDTO = FeeDecoder(RegulationJson["ccFees"], calculationBasis, unitList, RegulationJson["ccFeeCalculationBasis"]);
                            
                            foreach( var i in listFeeDecoderResponseDTO){

                                mapObject(LpcoRegulationId, AgencyID, // long LpcoRegulationId, short agencyID, int? unitID,
                                i.Unit, i.CalculationBasisValue,// int? unitID, int? calculationBasis,
                                calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ccFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),//int? calculationSource
                                MasterDocumentClassificationCode.CATCH_CERTIFICATE,//string masterDocumentClassificationCode
                                DocumentClassificationCode.CATCH_CERTIFICATE,//string documentClassificationCode,
                                i.QtyRangeTo, i.QtyRangeFrom, "PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                i.Rate, //decimal? rate
                                null, // decimal? minAmount
                                additionalAmount, //decimal? additionalAmount
                                null, //int? additionalAmountOn
                                UserRoleId );//int UserRoleId
                            }
                        }
                        else{
                            mapObject(LpcoRegulationId, AgencyID, null,// long LpcoRegulationId, short agencyID, int? unitID,
                            calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ccFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                            calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["ccFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? calculationSource
                            MasterDocumentClassificationCode.EXPORT_CERTIFICATE,//string masterDocumentClassificationCode
                            DocumentClassificationCode.EXPORT_CERTIFICATE,//string documentClassificationCode,
                            null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                            RegulationJson["ccFees"] == null ? null : Decimal.TryParse(getValue(RegulationJson["ccFees"]), out n1) ? (decimal?)n1:null,//decimal? rate
                            null, // decimal? minAmount
                            additionalAmount,//decimal? additionalAmount
                            null,//int? additionalAmountOn
                            UserRoleId );//int UserRoleId
                            
                        }
                    }
                }

                //for Export Certificate Fees
                feePropertyDetail = PropertyNameList.Where(x=>x.NameShort=="epFees").FirstOrDefault();
                
                if(RegulationJson.ContainsKey("epRequired")){
                    if(getLowerValue(RegulationJson["epRequired"]) == "yes"){

                        decimal? additionalAmount = AdditionalAmountAccumulator(PropertyNameList, RegulationJson, FEEClassificationCodeForAdditionalAmount.EXPORT_PERMIT);

                        if(feePropertyDetail.NameLong.Contains("[Quantity;Unit;Price|]")){

                            List<FeeDecoderResponseDTO> listFeeDecoderResponseDTO = new List<FeeDecoderResponseDTO>();

                            listFeeDecoderResponseDTO = FeeDecoder(RegulationJson["epFees"], calculationBasis, unitList, RegulationJson["epFeeCalculationBasis"]);
                            
                            foreach( var i in listFeeDecoderResponseDTO){

                                mapObject(LpcoRegulationId, AgencyID, // long LpcoRegulationId, short agencyID, int? unitID,
                                i.Unit, i.CalculationBasisValue,// int? unitID, int? calculationBasis,
                                calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["epFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(),//int? calculationSource
                                MasterDocumentClassificationCode.EXPORT_PERMIT,//string masterDocumentClassificationCode
                                DocumentClassificationCode.EXPORT_PERMIT,//string documentClassificationCode,
                                i.QtyRangeTo, i.QtyRangeFrom, "PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                                i.Rate, //decimal? rate
                                RegulationJson["epFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["epFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null,// decimal? minAmount
                                additionalAmount, //decimal? additionalAmount
                                RegulationJson["epFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["epFeeAdditionalAmountOn"])) ? null : 
                                        (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["epFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(), //int? additionalAmountOn
                                UserRoleId );//int UserRoleId
                            }
                        }
                        else{
                            mapObject(LpcoRegulationId, AgencyID, null,// long LpcoRegulationId, short agencyID, int? unitID,
                            calculationBasis.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["epFeeCalculationBasis"])).Select(x=>x.ID).FirstOrDefault(),// int? calculationBasis,
                            calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["epFeeCalculationSource"])).Select(x=>x.ID).FirstOrDefault(), //int? calculationSource
                            MasterDocumentClassificationCode.EXPORT_PERMIT,//string masterDocumentClassificationCode
                            DocumentClassificationCode.EXPORT_PERMIT,//string documentClassificationCode,
                            null,null,"PKR", // int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, 
                            RegulationJson["epFees"] == null ? null : Decimal.TryParse(getValue(RegulationJson["epFees"]), out n1) ? (decimal?)n1:null,//decimal? rate
                            RegulationJson["epFeeMinimumAmount"] != null ? Decimal.TryParse(getValue(RegulationJson["epFeeMinimumAmount"]), out n1) ? (decimal?) n1 : null : null, // decimal? minAmount
                            additionalAmount,//decimal? additionalAmount
                            RegulationJson["epFeeAdditionalAmountOn"] == null ? null : string.IsNullOrEmpty( getLowerValue(RegulationJson["epFeeAdditionalAmountOn"])) ? null : 
                                    (int?)calculationSource.Where(x=>x.Description.ToLower() == getLowerValue(RegulationJson["epFeeAdditionalAmountOn"])).Select(x=>x.ID).FirstOrDefault(),//int? additionalAmountOn
                            UserRoleId );//int UserRoleId


                            
                        }
                    }
                }

            }
        }

        private decimal? AdditionalAmountAccumulator(List<SheetAttributeMapping> PropertyNameList, JObject RegulationJson, string classificationCode){

            var additionalAmountDetail = PropertyNameList.Where(x=>x.ClassificationCode == classificationCode).ToList();
            if (additionalAmountDetail.Count ==0){
                return null;
            }
            else{
                decimal n1;
                decimal? totalAmount = null;
                foreach(var i in additionalAmountDetail){
                    var amount =RegulationJson[i.NameShort] != null? Decimal.TryParse(getValue(RegulationJson[i.NameShort]), out n1)? (decimal?)n1 : null : null;          
                    totalAmount = amount == null ? totalAmount : totalAmount == null ? amount : totalAmount + amount ;
                }
                return totalAmount;
            }
        }
        private List<FeeDecoderResponseDTO> FeeDecoder(JToken fees, List<CalculationBasis> calculationBasisList, List<PSW.ITT.Data.Entities.Ref_Units> unitList, JToken calculationBasis){
            
            decimal n1;
            int n2;
            List<FeeDecoderResponseDTO> ListResponseModel = new List<FeeDecoderResponseDTO>();
            List<string> record = getValueList(fees);//.Split('|');
            foreach( var i in record){

                FeeDecoderResponseDTO feeDecoderResponseDTO = new FeeDecoderResponseDTO();

                string[] seperator = i.Split(';');
                feeDecoderResponseDTO.Rate = Decimal.TryParse( seperator[2], out n1) ? (decimal?) n1:null;
                feeDecoderResponseDTO.Unit = unitList.Where(x=>x.Unit_Description.ToLower() == getLowerValue(seperator[1])).Select(x=>x.Unit_ID).FirstOrDefault();
                if(seperator[0].Contains("-")){
                    feeDecoderResponseDTO.QtyRangeTo = int.TryParse(seperator[0].Split('-').First(), out n2) ? (int?) n2:null;
                    feeDecoderResponseDTO.QtyRangeFrom = int.TryParse(seperator[0].Split('-').Last(), out n2) ?(int?) n2:null;
                    feeDecoderResponseDTO.CalculationBasisValue = calculationBasisList.Where(x=>x.Description.ToLower() == getLowerValue(calculationBasis)).Select(x=>x.ID).FirstOrDefault();
                }
                else{
                    feeDecoderResponseDTO.CalculationBasisValue = calculationBasisList.Where(x=>x.Description.ToLower() == "quantity").Select(x=>x.ID).FirstOrDefault();
                }
                ListResponseModel.Add(feeDecoderResponseDTO);
            }
            return(ListResponseModel);
        }
        private void mapObject(long LpcoRegulationId, short agencyID, int? unitID, int? calculationBasis, int? calculationSource, string masterDocumentClassificationCode, string documentClassificationCode,
                                int? qtyRangeTo, int? qtyRangeFrom, string currencyCode, decimal? rate, decimal? minAmount, decimal? additionalAmount, int? additionalAmountOn, int UserRoleId){
                                    
            LPCOFeeStructure lpcoFeeStructure = new LPCOFeeStructure();
            lpcoFeeStructure.AgencyID = agencyID;
            lpcoFeeStructure.Unit_ID = unitID == null ? null : unitID;
            lpcoFeeStructure.CalculationBasis = calculationBasis;
            lpcoFeeStructure.CalculationSource = calculationSource;
            lpcoFeeStructure.MasterDocumentClassificationCode = masterDocumentClassificationCode;
            lpcoFeeStructure.DocumentClassificationCode = documentClassificationCode;
            lpcoFeeStructure.QtyRangeTo = qtyRangeTo == null ? null :  qtyRangeTo;
            lpcoFeeStructure.QtyRangeFrom = qtyRangeFrom == null ? null :  qtyRangeFrom;
            lpcoFeeStructure.CurrencyCode = "PKR";
            lpcoFeeStructure.Rate = rate == null ? null :  rate;
            lpcoFeeStructure.MinAmount = minAmount == null ? null : minAmount;
            lpcoFeeStructure.AdditionalAmount = additionalAmount == null ? null : additionalAmount;
            lpcoFeeStructure.AdditionalAmountOn = additionalAmountOn == null ? null : additionalAmountOn;
            lpcoFeeStructure.IsActive = true;
            lpcoFeeStructure.LPCORegulationID = LpcoRegulationId;
            lpcoFeeStructure.CreatedOn = DateTime.Now;
            lpcoFeeStructure.UpdatedOn = DateTime.Now;
            lpcoFeeStructure.CreatedBy = UserRoleId;
            lpcoFeeStructure.UpdatedBy = UserRoleId;


            Command.UnitOfWork.LPCOFeeStructureRepository.Add(lpcoFeeStructure);
        }

         private List<string> getValueList(JToken str){
            List<string> list =  new List<string>();
            foreach( var i in str){
                list.Add(i.Value<string>());
            }
            return list;
        }
        private string getValue(JToken str){
            return str.Value<string>();
        }
        private string getLowerValue(JToken str){
            return str.Value<string>().ToLower();
        }
    }
    
}