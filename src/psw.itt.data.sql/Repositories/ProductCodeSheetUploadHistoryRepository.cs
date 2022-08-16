using System.Data;
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

        #endregion
    }
}
