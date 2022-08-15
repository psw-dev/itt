/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Data;
using psw.itt.data.Entities;
using psw.itt.data.IRepositories;

namespace psw.itt.data.sql.Repositories
{
    public class SheetAttributeMappingRepository : Repository<SheetAttributeMapping>, ISheetAttributeMappingRepository
    {
		#region public constructors

        public SheetAttributeMappingRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[SheetAttributeMapping]";
			PrimaryKeyName = "ID";
        }

		#endregion

		#region Public methods

		#endregion
    }
}
