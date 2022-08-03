using System.Data;
using psw.itt.data.Entities;
using psw.itt.data.IRepositories;

namespace psw.itt.data.sql.Repositories
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
