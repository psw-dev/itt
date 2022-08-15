
using System.ComponentModel;

namespace PSW.itt.Common.Enums
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
    }
}