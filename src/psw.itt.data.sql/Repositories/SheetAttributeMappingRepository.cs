/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class SheetAttributeMappingRepository : Repository<SheetAttributeMapping>, ISheetAttributeMappingRepository
    {
        #region public constructors

        public SheetAttributeMappingRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[SheetAttributeMapping]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods
        public List<SheetAttributeMapping> GetAgencyAttributeMapping(short TradeTranTypeID, short AgencyID)
        {
            var query = @"SELECT *
                        FROM [ITT].[dbo].[SheetAttributeMapping]
                        WHERE (TradeTranTypeID = @TRADETRANTYPEID
                        AND AgencyID = @AGENCYID) 
                        or (TradeTranTypeID is null
                        AND AgencyID is null)
                        AND SheetType = 'OGA'
                        AND IsActive = 1";

            return _connection.Query<SheetAttributeMapping>(
                    query, param: new { TRADETRANTYPEID = TradeTranTypeID, AGENCYID = AgencyID },
                    transaction: _transaction
                   ).ToList();
        }
        #endregion
    }
}
