using System;
using System.Collections.Generic;

namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the ProductCodeSheetUploadHistory table in the database 
    /// </summary>
	public class ProductCodeSheetUploadHistory : Entity
    {
        #region Fields

        private long _iD;
        private string _name;
        private long _attachedFileID;
        private short _productCodeSheetUploadStatusID;
        private int _totalRecordsCount;
        private int _disputedRecordsCount;
        private int _processedRecordsCount;
        private int _duplicateRecordsCount;
        private short _agencyID;
        private short _tradeTranTypeID;
        private bool _isCurrent;
        private string _processingResponse;
        private int _createdBy;
        private DateTime _createdOn;
        private int _updatedBy;
        private DateTime _updatedOn;

        #endregion

        #region Properties

        public long ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public long AttachedFileID { get { return _attachedFileID; } set { _attachedFileID = value; } }
        public short ProductCodeSheetUploadStatusID { get { return _productCodeSheetUploadStatusID; } set { _productCodeSheetUploadStatusID = value; } }
        public int TotalRecordsCount { get { return _totalRecordsCount; } set { _totalRecordsCount = value; } }
        public int ProcessedRecordsCount  { get { return _processedRecordsCount ; } set { _processedRecordsCount  = value; } }
        public int DisputedRecordsCount  { get { return _disputedRecordsCount ; } set { _disputedRecordsCount  = value; } }
        public bool IsCurrent { get { return _isCurrent; } set { _isCurrent = value; } }
        public int DuplicateRecordsCount { get { return _duplicateRecordsCount; } set { _duplicateRecordsCount = value; } }
        public short AgencyID  { get { return _agencyID ; } set { _agencyID  = value; } }
        public short TradeTranTypeID { get { return _tradeTranTypeID; } set { _tradeTranTypeID = value; } }
        public string ProcessingResponse { get { return _processingResponse; } set { _processingResponse = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
        public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value; } }
        public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value; } }

        #endregion

        #region Methods

        #endregion

        #region public Methods

        public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object>
            {
                {"ID", ID},
                {"Name", Name},
                {"AttachedFileID", AttachedFileID},
                {"ProductCodeSheetUploadStatusID", ProductCodeSheetUploadStatusID},
                {"TotalRecordsCount",TotalRecordsCount},
                {"DisputedRecordsCount",DisputedRecordsCount },
                {"ProcessedRecordsCount",ProcessedRecordsCount },
                {"IsCurrent",IsCurrent },
                {"DuplicateRecordsCount", DuplicateRecordsCount},
                {"AgencyID",AgencyID },
                {"TradeTranTypeID", TradeTranTypeID},
                {"ProcessingResponse", ProcessingResponse},
                {"CreatedBy", CreatedBy},
                {"CreatedOn", CreatedOn},
                {"UpdatedBy", UpdatedBy},
                {"UpdatedOn", UpdatedOn}
            };
        }
        public override object GetInsertUpdateParams()
        {
            return new
            {

                Name,
                AttachedFileID,
                ProductCodeSheetUploadStatusID,
                TotalRecordsCount,
                DisputedRecordsCount,
                IsCurrent,
                DuplicateRecordsCount,
                ProcessedRecordsCount,
                AgencyID,
                TradeTranTypeID,
                ProcessingResponse,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn
            };
        }

        #endregion

        #region Constructors
        public ProductCodeSheetUploadHistory()
        {
            TableName = "ProductCodeSheetUploadHistory";
            PrimaryKeyName = "ID";
        }
        #endregion
    }
}

