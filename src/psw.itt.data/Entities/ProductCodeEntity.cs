using System;
using System.Collections.Generic;

namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the ProductCode table in the database 
    /// </summary>
	public class ProductCodeEntity : Entity
    {
        #region Fields

        private long? _serialId;
        private long _iD;
        private string _hSCode;
        private string _hSCodeExt;
        private string _productCode;
        private long _productCodeChapterID;
        private string _chapterCode;
        private string _description;
        private long? _productCodeSheetUploadHistoryID;
        private short _tradeTranTypeID;
        private DateTime _effectiveFromDt;
        private DateTime _effectiveThruDt;
        private int _createdBy;
        private DateTime _createdOn;
        private int _updatedBy;
        private DateTime _updatedOn;

        #endregion

        #region Properties

        public long? SerialID { get { return _serialId; } set { _serialId = value; } }
        public long ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public string HSCode { get { return _hSCode; } set { _hSCode = value; } }
        public string HSCodeExt { get { return _hSCodeExt; } set { _hSCodeExt = value; } }
        public string ProductCode { get { return _productCode; } set { _productCode = value; } }
        public long ProductCodeChapterID { get { return _productCodeChapterID; } set { _productCodeChapterID = value; } }
        public string ChapterCode { get { return _chapterCode; } set { _chapterCode = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public short TradeTranTypeID { get { return _tradeTranTypeID; } set { _tradeTranTypeID = value; } }
        public long? ProductCodeSheetUploadHistoryID { get { return _productCodeSheetUploadHistoryID; } set { _productCodeSheetUploadHistoryID = value; } }
        public DateTime EffectiveFromDt { get { return _effectiveFromDt; } set { _effectiveFromDt = value; } }
        public DateTime EffectiveThruDt { get { return _effectiveThruDt; } set { _effectiveThruDt = value; } }
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
                {"HSCode", HSCode},
                {"HSCodeExt", HSCodeExt},
                {"ProductCode", ProductCode},
                {"ProductCodeChapterID", ProductCodeChapterID},
                {"ChapterCode", ChapterCode},
                {"Description", Description},
                {"ProductCodeSheetUploadHistoryID", ProductCodeSheetUploadHistoryID},
                {"TradeTranTypeID",TradeTranTypeID},
                {"EffectiveFromDt", EffectiveFromDt},
                {"EffectiveThruDt", EffectiveThruDt},
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

                HSCode,
                HSCodeExt,
                ProductCode,
                ProductCodeChapterID,
                ChapterCode,
                Description,
                ProductCodeSheetUploadHistoryID,
                TradeTranTypeID,
                EffectiveFromDt,
                EffectiveThruDt,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn
            };
        }

        #endregion

        #region Constructors
        public ProductCodeEntity()
        {
            TableName = "ProductCode";
            PrimaryKeyName = "ID";
        }
        #endregion
    }
}

