/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System;
using System.Collections.Generic;
using System.Linq;


namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the CalculationBasis table in the database 
    /// </summary>
	public class CalculationBasis : Entity
	{
		#region Fields
		
		private int _iD;
		private string _description;

		#endregion

		#region Properties
		
		public int ID { get { return _iD; } set { _iD = value; PrimaryKey = value; }}
		public string Description { get { return _description; } set { _description = value;  }}

		#endregion

		#region Methods

		#endregion

		#region public Methods

		public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object> 
			{
				{"ID", ID},
				{"Description", Description}
			};
        }

		#endregion

		#region Constructors
		 public CalculationBasis()
        {
            TableName = "CalculationBasis";
            PrimaryKeyName = "ID";
        }
		#endregion
	}
} 

