using System.Data;
using psw.itt.data.Entities;
using psw.itt.data.IRepositories;

namespace psw.itt.data.sql.Repositories
{
    public class ChapterAgencyLinkRepository : Repository<ChapterAgencyLink>, IChapterAgencyLinkRepository
    {
        #region public constructors

        public ChapterAgencyLinkRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ChapterAgencyLink]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods

        #endregion
    }
}
