using System;
using System.Collections.Generic;

namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the ProductRegulationRequirement table in the database 
    /// </summary>
	public class ProductRegulationRequirement : Entity
    {
        #region Fields

        private long _iD;
        private long _productCodeAgencyLinkID;
        private long _lPCORegulationID;
        private long _lPCOFeeStructureID;
        private DateTime _effectiveFromDt;
        private DateTime _effectiveThruDt;
        private int _createdBy;
        private DateTime _createdOn;
        private int _updatedBy;
        private DateTime _updatedOn;
        private short _tradeTranTypeID;

        #endregion

        #region Properties

        public long ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public long ProductCodeAgencyLinkID { get { return _productCodeAgencyLinkID; } set { _productCodeAgencyLinkID = value; } }
        public long LPCORegulationID { get { return _lPCORegulationID; } set { _lPCORegulationID = value; } }
        public long LPCOFeeStructureID { get { return _lPCOFeeStructureID; } set { _lPCOFeeStructureID = value; } }
        public DateTime EffectiveFromDt { get { return _effectiveFromDt; } set { _effectiveFromDt = value; } }
        public DateTime EffectiveThruDt { get { return _effectiveThruDt; } set { _effectiveThruDt = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
        public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value; } }
        public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value; } }
        public short TradeTranTypeID { get { return _tradeTranTypeID; } set { _tradeTranTypeID = value; } }


        #endregion

        #region Methods

        #endregion

        #region public Methods

        public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object>
            {
                {"ID", ID},
                {"ProductCodeAgencyLinkID", ProductCodeAgencyLinkID},
                {"LPCORegulationID", LPCORegulationID},
                {"LPCOFeeStructureID", LPCOFeeStructureID},
                {"EffectiveFromDt", EffectiveFromDt},
                {"EffectiveThruDt", EffectiveThruDt},
                {"CreatedBy", CreatedBy},
                {"CreatedOn", CreatedOn},
                {"UpdatedBy", UpdatedBy},
                {"UpdatedOn", UpdatedOn},
                {"TradeTranTypeID",TradeTranTypeID}
            };
        }
        public override object GetInsertUpdateParams()
        {
            return new
            {

                ProductCodeAgencyLinkID,
                LPCORegulationID,
                LPCOFeeStructureID,
                EffectiveFromDt,
                EffectiveThruDt,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn,
                TradeTranTypeID
            };
        }

        #endregion

        #region Constructors
        public ProductRegulationRequirement()
        {
            TableName = "ProductRegulationRequirement";
            PrimaryKeyName = "ID";
        }
        #endregion
    }
}

