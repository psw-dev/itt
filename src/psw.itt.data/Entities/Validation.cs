/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System;
using System.Collections.Generic;
using System.Linq;


namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the Validation table in the database 
    /// </summary>
	public class Validation : Entity
	{
		#region Fields
		
		private int _iD;
		private string _description;
		private string _value;
		private DateTime _createdOn;
		private int _createdBy;
		private DateTime _updatedOn;
		private int _updatedBy;

		#endregion

		#region Properties
		
		public int ID { get { return _iD; } set { _iD = value; PrimaryKey = value; }}
		public string Description { get { return _description; } set { _description = value;  }}
		public string Value { get { return _value; } set { _value = value;  }}
		public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value;  }}
		public int CreatedBy { get { return _createdBy; } set { _createdBy = value;  }}
		public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value;  }}
		public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value;  }}

		#endregion

		#region Methods

		#endregion

		#region public Methods

		public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object> 
			{
				{"ID", ID},
				{"Description", Description},
				{"Value", Value},
				{"CreatedOn", CreatedOn},
				{"CreatedBy", CreatedBy},
				{"UpdatedOn", UpdatedOn},
				{"UpdatedBy", UpdatedBy}
			};
        }
 		public override object GetInsertUpdateParams()
        {
            return new
            {
				ID,
				Description,
				Value,
				CreatedOn,
				CreatedBy,
				UpdatedOn,
				UpdatedBy
			};
        }

		#endregion

		#region Constructors
		
		#endregion
	}
} 

