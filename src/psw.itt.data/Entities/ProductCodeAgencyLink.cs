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
        private DateTime _regulationEffectiveFromDt;
        private DateTime _regulationEffectiveThruDt;
        private int _createdBy;
        private DateTime _createdOn;
        private int _updatedBy;
        private DateTime _updatedOn;
        private bool _isActive;
        private bool _softDelete;

        #endregion

        #region Properties

        public long ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public long ProductCodeID { get { return _productCodeID; } set { _productCodeID = value; } }
        public short AgencyID { get { return _agencyID; } set { _agencyID = value; } }
        public DateTime EffectiveFromDt { get { return _effectiveFromDt; } set { _effectiveFromDt = value; } }
        public DateTime EffectiveThruDt { get { return _effectiveThruDt; } set { _effectiveThruDt = value; } }
        public DateTime RegulationEffectiveFromDt { get { return _regulationEffectiveFromDt; } set { _regulationEffectiveFromDt = value; } }
        public DateTime RegulationEffectiveThruDt { get { return _regulationEffectiveThruDt; } set { _regulationEffectiveThruDt = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
        public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value; } }
        public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value; } }
        public bool IsActive { get { return _isActive; } set { _isActive = value; } }
        public bool SoftDelete { get { return _softDelete; } set { _softDelete = value; } }


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
                {"RegulationEffectiveFromDt", RegulationEffectiveFromDt},
                {"RegulationEffectiveThruDt", RegulationEffectiveThruDt},
                {"CreatedBy", CreatedBy},
                {"CreatedOn", CreatedOn},
                {"UpdatedBy", UpdatedBy},
                {"UpdatedOn", UpdatedOn},
                {"IsActive", IsActive},
                {"SoftDelete", SoftDelete},
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
                RegulationEffectiveFromDt,
                RegulationEffectiveThruDt,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn,
                SoftDelete
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

