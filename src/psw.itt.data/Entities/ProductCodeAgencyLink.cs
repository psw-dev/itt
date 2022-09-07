using System;
using System.Collections.Generic;

namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the ProductCodeAgencyLink table in the database 
    /// </summary>
	public class ProductCodeAgencyLink : Entity
    {
        #region Fields

        private long _iD;
        private long _productCodeID;
        private short _agencyID;
        private DateTime _effectiveFromDt;
        private DateTime _effectiveThruDt;
        private int _createdBy;
        private DateTime _createdOn;
        private int _updatedBy;
        private DateTime _updatedOn;

        #endregion

        #region Properties

        public long ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public long ProductCodeID { get { return _productCodeID; } set { _productCodeID = value; } }
        public short AgencyID { get { return _agencyID; } set { _agencyID = value; } }
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
                {"ProductCodeID", ProductCodeID},
                {"AgencyID", AgencyID},
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

                ProductCodeID,
                AgencyID,
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
        public ProductCodeAgencyLink()
        {
            TableName = "ProductCodeAgencyLink";
            PrimaryKeyName = "ID";
        }
        #endregion
    }
}

