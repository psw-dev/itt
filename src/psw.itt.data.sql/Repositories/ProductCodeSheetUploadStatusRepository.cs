using System.Data;
using psw.itt.data.Entities;
using psw.itt.data.IRepositories;

namespace psw.itt.data.sql.Repositories
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
