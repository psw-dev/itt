using System;
using System.Collections.Generic;

namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the ProductCodeSheetUploadStatus table in the database 
    /// </summary>
	public class ProductCodeSheetUploadStatus : Entity
    {
        #region Fields

        private short _iD;
        private string _name;
        private int _createdBy;
        private DateTime _createdOn;

        #endregion

        #region Properties

        public short ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
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
                {"CreatedBy", CreatedBy},
                {"CreatedOn", CreatedOn}
            };
        }
        public override object GetInsertUpdateParams()
        {
            return new
            {
                Name,
                CreatedBy,
                CreatedOn
            };
        }

        #endregion

        #region Constructors
        public ProductCodeSheetUploadStatus()
        {
            TableName = "ProductCodeSheetUploadStatus";
            PrimaryKeyName = "ID";
        }
        #endregion
    }
}

