/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Data;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class LPCOFeeStructureRepository : Repository<LPCOFeeStructure>, ILPCOFeeStructureRepository
    {
		#region public constructors

        public LPCOFeeStructureRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[LPCOFeeStructure]";
			PrimaryKeyName = "ID";
        }

		#endregion

		#region Public methods

		#endregion
    }
}
