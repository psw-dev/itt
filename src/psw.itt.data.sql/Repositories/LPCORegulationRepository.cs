using System.Data;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class LPCORegulationRepository : Repository<LPCORegulation>, ILPCORegulationRepository
    {
        #region public constructors

        public LPCORegulationRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[LPCORegulation]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods

        #endregion
    }
}
