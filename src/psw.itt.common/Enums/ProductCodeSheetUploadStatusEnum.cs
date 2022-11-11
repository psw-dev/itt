
using System.ComponentModel;

namespace PSW.ITT.Common.Enums
{
    public enum ProductCodeSheetUploadStatusEnum
    {

        [Description("In Progress")]
        IN_PROGRESS = 1,
        [Description("Processed")]
        PROCESSED = 2,
        [Description("Failed")]
        FAILED = 3,
        [Description("Cancelled")]
        CANCELLED = 4,
        [Description("Validated")]
        VALIDATED = 5,
        [Description("Structure Validation Failed")]
        STRUCTURE_VALIDATION_FAILED = 6,

        [Description("File Column Validated")]
        FILE_COLUMN_VALIDATED = 7,
        [Description("Column Value Validation Failed")]
        FILE_COLUMN_VALUE_VALIDATION_FAILED = 8,
    }
}