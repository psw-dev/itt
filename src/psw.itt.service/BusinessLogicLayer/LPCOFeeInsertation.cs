using System;
using System.Linq;
using System.Collections.Generic;
using PSW.ITT.Data.Entities;
using PSW.ITT.Service.DTO;

namespace PSW.ITT.service
{
    public class LPCOFeeInsertation
    {
        // private command Command { get; set; }
        // private JObject RegulationJson { get; set; }
        // private List<SheetAttributeMapping> propertyNameList { get; set; }

        // public LPCOFeeInsertation(LPCOFeeCleanResp lpcoFeeEntity, GetDocumentRequirementRequest request)
        // {
        //     this.LPCOFeeEntity = lpcoFeeEntity;
        //     this.Request.Add(request);
        // }

        // public LPCOFeeCalculator(LPCOFeeCleanResp lpcoFeeEntity, List<GetDocumentRequirementRequest> request)
        // {
        //     this.LPCOFeeEntity = lpcoFeeEntity;
        //     this.Request = request;
        // }

        // public LPCOFeeDTO Calculate()
        // {
        //     LPCOFeeDTO calculatedFee = new LPCOFeeDTO();
        //     calculatedFee.Fee = 0m;
        //     calculatedFee.AdditionalAmount = 0m;
        //     calculatedFee.AdditionalAmountOn = string.Empty;

        //     if(Request == null || Request.Count <= 0)
        //     {
        //         return calculatedFee;
        //     }

        //     var calculationBasis = LPCOFeeEntity.CalculationBasis;
        //     var calculationSource = LPCOFeeEntity.CalculationSource;

        //     switch(calculationBasis)
        //     {
        //         case "Quantity":
        //             break;
        //         case "Fixed":
        //             break;
        //         case "AdVal":
        //             if(calculationSource.ToLower() == "item")
        //             {
        //                 calculatedFee = CalculateItemFeeAsAdVal(Request[0]);     // Handled only for PSQCA right now
        //             }
        //             else if (calculationSource.ToLower() == "document")
        //             {
        //                 calculatedFee = CalculateDocumentFeeAsAdVal();     // Handled only for PSQCA right now
        //             }
        //             break;
        //         default:
        //             break;
        //     }

        //     return calculatedFee;
        // }

        // // Handled only for PSQCA right now
        // private LPCOFeeDTO CalculateItemFeeAsAdVal(GetDocumentRequirementRequest request)
        // {
        //     LPCOFeeDTO lpcoFee = new LPCOFeeDTO();
        //     var result = 0m;
        //     var percentage = LPCOFeeEntity.Rate == null ? 0m : (LPCOFeeEntity.Rate/100);
        //     var minAmount = LPCOFeeEntity.MinAmount ?? 0m;
        //     var additionalAmount = LPCOFeeEntity.AdditionalAmount ?? 0m;
        //     var additionalAmountOn = LPCOFeeEntity.AdditionalAmountOn ?? string.Empty;

        //     var percentageOfValue = percentage * request.ImportExportValue;

        //     if(percentageOfValue > minAmount)
        //     {
        //         result = (decimal)(percentageOfValue);
        //     }
        //     else
        //     {
        //         result = (decimal)(minAmount);
        //     }

        //     lpcoFee.Fee = Math.Round(result);
        //     lpcoFee.AdditionalAmount = Math.Round(additionalAmount);
        //     lpcoFee.AdditionalAmountOn = additionalAmountOn;

        //     return lpcoFee;
        // }

        // // Handled only for PSQCA right now
        // private LPCOFeeDTO CalculateDocumentFeeAsAdVal()
        // {
        //     LPCOFeeDTO lpcoFee = new LPCOFeeDTO();
        //     var result = 0m;
        //     var percentage = LPCOFeeEntity.Rate == null ? 0m : (LPCOFeeEntity.Rate/100);
        //     var minAmount = LPCOFeeEntity.MinAmount ?? 0m;
        //     var additionalAmount = LPCOFeeEntity.AdditionalAmount ?? 0m;
        //     var additionalAmountOn = LPCOFeeEntity.AdditionalAmountOn ?? string.Empty;

        //     var percentageOfValue = percentage * Request.Sum(x => x.ImportExportValue);

        //     if(percentageOfValue > minAmount)
        //     {
        //         result = (decimal)(percentageOfValue);
        //     }
        //     else
        //     {
        //         result = (decimal)(minAmount);
        //     }

        //     lpcoFee.Fee = Math.Round(result);
        //     lpcoFee.AdditionalAmount = Math.Round(additionalAmount);
        //     lpcoFee.AdditionalAmountOn = additionalAmountOn;

        //     return lpcoFee;
        // }

        // private decimal CalculateAsFixed(LPCOFeeStructure lpcoFeeEntity)
        // {
        //     var result = 0m;
        //     return result;
        // }

        // private decimal CalculateAsQuantity(LPCOFeeStructure lpcoFeeEntity)
        // {
        //     var result = 0m;
        //     return result;
        // }
    }
}