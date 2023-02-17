/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System;
using System.Collections.Generic;


namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the DocumentType table in the database 
    /// </summary>
	public class DocumentType : Entity
    {
        #region Fields

        private string _code;
        private string _name;
        private short? _agencyID;
        private System.SByte _attachedObjectFormatID;
        private bool _sysOwn;
        private string _dBTableName;
        private DateTime _createdOn;
        private int _createdBy;
        private DateTime _updatedOn;
        private int _updatedBy;
        private string _documentClassificationCode;

        #endregion

        #region Properties

        public string Code { get { return _code; } set { _code = value; PrimaryKey = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public short? AgencyID { get { return _agencyID; } set { _agencyID = value; } }
        public System.SByte AttachedObjectFormatID { get { return _attachedObjectFormatID; } set { _attachedObjectFormatID = value; } }
        public bool SysOwn { get { return _sysOwn; } set { _sysOwn = value; } }
        public string DBTableName { get { return _dBTableName; } set { _dBTableName = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value; } }
        public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value; } }
        public string DocumentClassificationCode { get { return _documentClassificationCode; } set {_documentClassificationCode = value; } }
 
        #endregion

        #region Methods

        #endregion

        #region public Methods

        public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object>
            {
                {"Code", Code},
                {"Name", Name},
                {"AgencyID", AgencyID},
                {"AttachedObjectFormatID", AttachedObjectFormatID},
                {"SysOwn", SysOwn},
                {"DBTableName", DBTableName},
                {"CreatedOn", CreatedOn},
                {"CreatedBy", CreatedBy},
                {"UpdatedOn", UpdatedOn},
                {"UpdatedBy", UpdatedBy},
                {"DocumentClassificationCode", DocumentClassificationCode}
            };
        }

        #endregion

        #region Constructors

        #endregion
    }
}

