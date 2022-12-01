using System.Data;
using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ProductCodeSheetUploadHistoryRepository : Repository<ProductCodeSheetUploadHistory>, IProductCodeSheetUploadHistoryRepository
    {
        #region public constructors

        public ProductCodeSheetUploadHistoryRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ProductCodeSheetUploadHistory]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods
public void SetIsCurrent(short AgencyID)
        {
             var query = @"UPDATE a
             SET a.isCurrent = 0 
             FROM [dbo].[ProductCodeSheetUploadHistory] a WHERE a.AgencyID=@AGENCYID;";
             _connection.Query<string>(
                    query ,param: new {  AGENCYID = AgencyID }
                   );
        }
        #endregion
    }
}
