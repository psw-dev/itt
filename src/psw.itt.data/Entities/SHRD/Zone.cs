/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System;
using System.Collections.Generic;



namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the Zone table in the database 
    /// </summary>
	public class Zone : Entity
    {
        #region Fields

        private short _iD;
        private string _name;
        private short _agencyID;
        private int? _cityID;
        private bool _softDeleted;
        private DateTime _createdOn;
        private int _createdBy;
        private DateTime _updatedOn;
        private int _updatedBy;
        private DateTime? _deletedOn;
        private int? _deletedBy;

        #endregion

        #region Properties

        public short ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public short AgencyID { get { return _agencyID; } set { _agencyID = value; } }
        public int? CityID { get { return _cityID; } set { _cityID = value; } }
        public bool SoftDeleted { get { return _softDeleted; } set { _softDeleted = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value; } }
        public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value; } }
        public DateTime? DeletedOn { get { return _deletedOn; } set { _deletedOn = value; } }
        public int? DeletedBy { get { return _deletedBy; } set { _deletedBy = value; } }

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
                {"AgencyID", AgencyID},
                {"CityID", CityID},
                {"SoftDeleted", SoftDeleted},
                {"CreatedOn", CreatedOn},
                {"CreatedBy", CreatedBy},
                {"UpdatedOn", UpdatedOn},
                {"UpdatedBy", UpdatedBy},
                {"DeletedOn", DeletedOn},
                {"DeletedBy", DeletedBy}
            };
        }

        #endregion

        #region Constructors

        #endregion
    }
}

