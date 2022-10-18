using System.Data;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ProductRegulationRequirementRepository : Repository<ProductRegulationRequirement>, IProductRegulationRequirementRepository
    {
        #region public constructors

        public ProductRegulationRequirementRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ProductRegulationRequirement]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods

        #endregion
    }
}
