using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ShrdCommonForLovRepository : SHRDRepository, IShrdCommonForLovRepository
    {
        #region public constructors

        public ShrdCommonForLovRepository(IDbConnection context) : base(context)
        {
          
        }

        #endregion

        #region Public methods
 public List<(int,string)> GetLOV(string TableName, string ColumnName )
        {
            var query = @"SELECT @CLOUMNNAME
                        FROM [dbo].[@TABLENAME]
                        WHERE IsActive = 1";

            return _connection.Query<(int,string)>(
                    query, param: new { TABLENAME = TableName, CLOUMNNAME = ColumnName }
                   ).ToList();
        }
        #endregion
    }
}
