using System;
using System.Collections.Generic;

namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the LPCORegulation table in the database 
    /// </summary>
	public class LPCORegulation : Entity
    {
        #region Fields

        private long _iD;
        private short _agencyID;
        private string _regulationJson;
        private string _hSCode;
        private string _hSCodeExt;
        private long _productCodeAgencyLinkID;
        private DateTime _effectiveFromDt;
        private DateTime _effectiveThruDt;
        private short _tradeTranTypeID;
        private int _createdBy;
        private DateTime _createdOn;
        private int _updatedBy;
        private DateTime _updatedOn;
        private string _hsCode;
        private string _hsCodeExt;
        private string _factor;
        private long _productCodeAgencyLinkID;
        private DateTime _effectiveThruDt;
        private DateTime _effectiveFromDt;
        private short _tradeTranTypeID;

        #endregion

        #region Properties


        public long ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public short AgencyID { get { return _agencyID; } set { _agencyID = value; } }
        public string HSCode { get { return _hSCode; } set { _hSCode = value; } }
        public string HSCodeExt { get { return _hSCodeExt; } set { _hSCodeExt = value; } }
        public string RegulationJson { get { return _regulationJson; } set { _regulationJson = value; } }
        public long ProductCodeAgencyLinkID { get { return _productCodeAgencyLinkID; } set { _productCodeAgencyLinkID = value; } }
        public DateTime EffectiveFromDt { get { return _effectiveFromDt; } set { _effectiveFromDt = value; } }
        public DateTime EffectiveThruDt { get { return _effectiveThruDt; } set { _effectiveThruDt = value; } }
        public short TradeTranTypeID { get { return _tradeTranTypeID; } set { _tradeTranTypeID = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
        public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value; } }
        public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value; } }
        public string HsCode { get { return _hsCode; } set { _hsCode = value; } }
        public string HsCodeExt { get { return _hsCodeExt; } set { _hsCodeExt = value; } }
        public string Factor { get { return _factor; } set { _factor = value; } }
        public long ProductCodeAgencyLinkID { get { return _productCodeAgencyLinkID; } set { _productCodeAgencyLinkID = value; } }
        public DateTime EffectiveThruDt { get { return _effectiveFromDt; } set { _effectiveFromDt = value; } }
        public DateTime EffectiveFromDt { get { return _effectiveThruDt; } set { _effectiveThruDt = value; } }
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
                {"HSCode", HSCode},
                {"HSCodeExt", HSCodeExt},
                {"AgencyID", AgencyID},
                {"RegulationJson", RegulationJson},
                {"ProductCodeAgencyLinkID", ProductCodeAgencyLinkID},
                {"EffectiveFromDt", EffectiveFromDt},
                {"EffectiveThruDt", EffectiveThruDt},
                {"TradeTranTypeID",TradeTranTypeID},
                {"CreatedBy", CreatedBy},
                {"CreatedOn", CreatedOn},
                {"UpdatedBy", UpdatedBy},
                {"UpdatedOn", UpdatedOn},
                {"HsCode", HsCode},
                {"HsCodeExt", HsCodeExt},
                {"Factor", Factor},
                {"ProductCodeAgencyLinkID", ProductCodeAgencyLinkID},
                {"EffectiveThruDt", EffectiveThruDt},
                {"EffectiveFromDt", EffectiveFromDt},
                {"TradeTranTypeID", TradeTranTypeID},
            };
        }
        public override object GetInsertUpdateParams()
        {
            return new
            {
                HSCode,
                HSCodeExt,
                AgencyID,
                RegulationJson,
                ProductCodeAgencyLinkID,
                EffectiveFromDt,
                EffectiveThruDt,
                TradeTranTypeID,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn,
                HsCode,
                HsCodeExt,
                Factor,
                ProductCodeAgencyLinkID,
                EffectiveThruDt,
                EffectiveFromDt,
                TradeTranTypeID,
            };
        }

        #endregion

        #region Constructors
        public LPCORegulation()
        {
            TableName = "LPCORegulation";
            PrimaryKeyName = "ID";
        }
        #endregion
    }
}

