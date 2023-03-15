/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Data;
using System;
using Dapper;
using System.Linq;
using PSW.ITT.Data.Entities;
using  PSW.ITT.Data.IRepositories;
using System.Collections.Generic;
using PSW.ITT.Data.DTO;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class AttachedObjectFormatRepository : Repository<AttachedObjectFormat>, IAttachedObjectFormatRepository
    {
        #region public constructors

        public AttachedObjectFormatRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[AttachedObjectFormat]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods

        #endregion
    }
}
