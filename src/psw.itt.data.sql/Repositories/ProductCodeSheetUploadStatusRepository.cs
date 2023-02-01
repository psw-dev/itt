using System.Data;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ProductCodeSheetUploadStatusRepository : Repository<ProductCodeSheetUploadStatus>, IProductCodeSheetUploadStatusRepository
    {
        #region public constructors

        public ProductCodeSheetUploadStatusRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ProductCodeSheetUploadStatus]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods

        #endregion
    }
}
