
using System.ComponentModel;

namespace PSW.ITT.Common.Enums
{
    public enum FileTypeEnum
    {
        [Description("Add Regulations")]
        ADD_REGULATIONS_TEMPLATE = 1,
        
        [Description("Update Regulations")]
        UPDATE_REGULATIONS_TEMPLATE = 2,
        
        [Description("Inactivate Regulations")]
        INACTIVATE_REGULATIONS_TEMPLATE = 3,

        [Description("Active product with agency association report")]
        ACTIVE_PRODUCT_WITH_AGENCY_ASSOCIATION_REPORT = 4,
        
        [Description("Validate Product Codes Template")]
        VALIDATE_PRODUCTCODE_TEMPLETE = 5,
        
        [Description("Validate Product Codes Report")]
        VALIDATE_PRODUCTCODE_REPORT = 6,
        
        [Description("Add Product Codes")]
        ADD_PRODUCTCODE_TEMPLATE = 7,
        
       
    }
}