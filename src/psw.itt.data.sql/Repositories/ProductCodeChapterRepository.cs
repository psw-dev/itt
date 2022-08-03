using System.Data;
using psw.itt.data.Entities;
using psw.itt.data.IRepositories;

namespace psw.itt.data.sql.Repositories
{
    public class ProductCodeChapterRepository : Repository<ProductCodeChapter>, IProductCodeChapterRepository
    {
        #region public constructors

        public ProductCodeChapterRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ProductCodeChapter]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods

        #endregion
    }
}
