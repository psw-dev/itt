using System.Data;
using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;
using System.Linq;
using System.Collections.Generic;

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
        public void SetIsCurrent(List<int> SheetTypeIDs)
        {
             var query = @"UPDATE a
             SET a.isCurrent = 0 
             FROM [dbo].[ProductCodeSheetUploadHistory] a WHERE a.SheetType in @SHEETTYPEID;";
             _connection.Query<string>(
                    query ,param: new {  SHEETTYPEID = SheetTypeIDs }
                   );
        }
        public List<ProductCodeSheetUploadHistory> GetFilesBySheetType(int AgencyId, List<int> SheetTypeIDs)
        {
             var query = @"SELECT *
             FROM [dbo].[ProductCodeSheetUploadHistory] a WHERE a.SheetType in @SHEETTYPEID AND AgencyId = AGENCYID  AND isCurrent =1;";
            return _connection.Query<ProductCodeSheetUploadHistory>(
                    query ,param: new {  SHEETTYPEID = SheetTypeIDs , AGENCYID = AgencyId},
                    transaction: _transaction
                   ).ToList();
                   
        }
        public List<ProductCodeSheetUploadHistory> GetFilesHistoryBySheetType(List<int> SheetTypeIDs)
        {
             var query = @"SELECT *
             FROM [dbo].[ProductCodeSheetUploadHistory] a WHERE a.SheetType in @SHEETTYPEID
             ORDER BY UpdatedOn DESC;";
            return _connection.Query<ProductCodeSheetUploadHistory>(
                    query ,param: new {  SHEETTYPEID = SheetTypeIDs },
                    transaction: _transaction
                   ).ToList();
        }
        #endregion
    }
}
