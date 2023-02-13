using System.Collections.Generic;
using System.Text.Json.Serialization;
using PSW.ITT.Data.DTO;

namespace PSW.ITT.Service.DTO
{
    public class GetDocumentRequirementResponse
    {
        [JsonPropertyName("isLPCORequired")]
        public bool isLPCORequired { get; set; }

        [JsonPropertyName("formNumber")]
        public string FormNumber { get; set; }

        [JsonPropertyName("documentaryRequirementList")]
        public List<DocumentaryRequirement> DocumentaryRequirementList { get; set; }
        public FinancialRequirement FinancialRequirement { get; set; }
        public ValidityRequirement ValidityRequirement { get; set; }

        [JsonPropertyName("allowedQuantity")]
        public string AllowedQuantity { get; set; }
    }

    public class DocumentaryRequirement
    {
        //Common for all Requirements

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("requirementType")]
        public string RequirementType { get; set; }

        [JsonPropertyName("documentTypeCode")]
        public string DocumentTypeCode { get; set; }

        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }

        [JsonPropertyName("attachedObjectFormatID")]
        public int AttachedObjectFormatID { get; set; }
    }

    public class FinancialRequirement
    {

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("plainAmount")]
        public string PlainAmount { get; set; }

        [JsonPropertyName("ammendmentFee")]
        public string AmmendmentFee { get; set; }

        [JsonPropertyName("plainAmmendmentFee")]
        public string PlainAmmendmentFee { get; set; }

        [JsonPropertyName("extensionFee")]
        public string ExtensionFee { get; set; }

        [JsonPropertyName("plainExtensionFee")]
        public string PlainExtensionFee { get; set; }

        [JsonPropertyName("additionalAmount")]
        public decimal AdditionalAmount { get; set; }

        [JsonPropertyName("additionalAmountOn")]
        public string AdditionalAmountOn { get; set; }
    }

    public class ValidityRequirement
    {
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("uomName")]
        public string UomName { get; set; }
        
        [JsonPropertyName("extensionAllowed")]
        public bool ExtensionAllowed { get; set; }

        [JsonPropertyName("extensionPeriod")]
        public int ExtensionPeriod { get; set; }

        [JsonPropertyName("extensionPeriodUnitName")]
        public string ExtensionPeriodUnitName { get; set; }
    }
}