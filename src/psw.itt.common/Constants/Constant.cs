namespace PSW.ITT.Common.Constants
{
    public static class Constant
    {
        public static string ActiveCountry = "ActiveCountry";
        public static string SendInboxMessageMethodId = "1330";
        public static string GenerateVoucherMethodId = "1416";
        public static string GetRequirementsMethodId = "1722";
        public static string GetFileDetails = "1214";

    }

    public enum IRMSChannel
    {
        GREEN,
        YELLOW,
        RED,
    }

    public enum AttachmentStatusEnum
    {
        REQUESTED = 1,
        COMPLETED = 2

    }

    public struct AltCode
    {
        public const string CERTIFICATE = "C";
        public const string REQUEST = "R";
        public const string INTERIM = "I";
    }

    public enum SearchByDppDashboard
    {
        YEAR = 0,
        MONTH = 1,
        WEEK = 2,
        DAY = 3,

    }

    // public struct AltCode
    // {
    //     public const string CERTIFICATE = "C";
    //     public const string REQUEST = "R";
    // }
    public enum TradeTranType
    {
        IMPORT = 1,
        EXPORT = 2,
        TRANSIT = 3
    }

    public static class DocumentClassificationCode
    {
        public const string EXPORT_CERTIFICATE = "EC";
        public const string RELEASE_ORDER = "RO";
        public const string IMPORT_PERMIT = "IMP";
        public const string SINGLE_DECLARATION = "SD";
        public const string PRODUCT_REGISTRATION = "PRD";
        public const string PREMISE_REGISTRATION = "PRM";
        public const string BUSINESS_REGISTRATION = "BSS";
        
    }

    public static class FileContentTypes
    {
        public const string PDF = "application/pdf";
    }
    public static class LabPaymentStatus
    {
        public const string PAID = "Paid";
        public const string UNPAID = "UnPaid";
    }

    public struct DocumentRegistrationCode
    {
        public const string PRODUCT = "PRD";
        public const string PREMISES = "PRM";
        public const string BUSINESS = "BSS";
    }

    public struct ReleaseOrderReviewStatus
    {
        public const int PENDING = 1;
        public const int COMPLETED = 2;
    }
    public static class DocumentNames
    {
        public const string EXPORT_CERTIFICATE = "Export Certificate";
        public const string RELEASE_ORDER = "Release Order";
        public const string IMPORT_PERMIT = "Import Permit";
    }
    public static class DateFormates
    {
        public const string DD_MM_YYYY = "dd/MM/yyyy";
    }
    public enum TransactionType
    {
        Credit = 1,
        Debit = 2
    }
    public struct ReplyCode
    {
        public const string OK = "200";
        public const string BadRequest = "400";
        public const string NotFound = "404";
        public const string InternalServerError = "500";
        public const string UnAuthorized = "401";
    }
    public struct DebitingMode
    {
        public const string Auto = "Auto";
        public const string Manual = "Manual";
    }

    public struct WeBOCResponseCode
    {
        public const long Succcess = 2001;
    }

    public struct ServiceMethod
    {
        // SD
        public const string SD_GET_RELEASE_ORDER_INFO = "1983";
        public const string SD_GET_IGM_INFO = "1915";
        public const string SD_GET_BLVIR_INFO = "1986";
        public const string SD_GET_EXPORT_CERTIFICATE_INFO = "1989";
        public const string SD_GET_IMPORTER_EXPORTER_INFO = "1981";
        public const string SD_VOUCHER_UPDATE = "19AA";
        public const string SD_GET_INFORMATION_BY_SDID = "19P1"; // Used for PSQCA
        public const string SD_GET_GD_INFORMATION_BY_SDID = "1923";
        public const string SD_GET_ATTACHED_IMPORT_PERMIT_BY_SDID = "194B";

        //UMS
        public const string UMS_GET_DOCUMENT_ASSIGNEE = "1257";
        public const string UMS_GET_DOCUMENT_ASSIGNEE_INFORMATION = "1260";
        public const string UMS_GET_ASSIGNED_USER_INFORMATION = "1264";

        //CLM
        public const string CLM_UPDATE_OGA_STATUS = "9002";

        //UPS
        public static string UPS_GENERATE_VOUCHER = "1416";

        //TARP
        public static string TARP_GET_FORM_NUMBER = "1730";

        //WFS
        public static string WFS_Create_Process_Instance = "1817";

    }

    public enum RegulationAction {
    ADD_REGULATION = 1,
    UPDATE_REGULATION = 2,
    INACTIVATE_REGULATION = 3
    }
    
    public enum ActionID {
    CANCEL = 1,
    SUBMIT = 2,
    VALIDATE = 3
}
 public static class ErrorCategories
    {
        #region Public Fields

        public const string Authorization = "Authorization";
        public const string BadRequest = "BadRequest";
        public const string BusinessRule = "BusinessRule";
        public const string Identification = "Identification";
        public const string InternalServer = "InternalServer";
        public const string NotFound = "NotFound";
        public const string ServiceUnavailable = "ServiceUnavailable";
        public const string Validation = "Validation";

        #endregion Public Fields
    }

    public static class TradePurpose{
        public const string ScreeningResearchTrial = "screening / research/ trial";
    }
     public static class Ref_Units
    {
        public const string Piece = "APT";
        public const string Cage = "CGE";
        public const string Box = "BX";
        public const string MuttonCarcass = "MC";
        public const string BeefQuartrer = "BQ";
        public const string PackingUnits = "PU";
    }
}