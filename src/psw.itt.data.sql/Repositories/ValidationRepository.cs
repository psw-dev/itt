/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Data;
using System;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;
using System.Collections.Generic;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ValidationRepository : Repository<Validation>, IValidationRepository
    {
		#region public constructors

        public ValidationRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[Validation]";
			PrimaryKeyName = "ID";
        }

		#endregion

		#region Public methods

		#endregion
    }
}
