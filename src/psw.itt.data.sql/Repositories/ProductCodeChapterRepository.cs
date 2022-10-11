using System.Data;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ProductCodeChapterRepository : Repository<ProductCodeChapter>, IProductCodeChapterRepository
    {
        #region public constructors

        public ProductCodeChapterRepository(IDbConnection context) : base(context)
        {
            TableName = "[SHRD].[dbo].[ProductCodeChapter]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods

        #endregion
    }
}
