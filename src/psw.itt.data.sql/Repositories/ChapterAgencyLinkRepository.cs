using System.Data;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
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
