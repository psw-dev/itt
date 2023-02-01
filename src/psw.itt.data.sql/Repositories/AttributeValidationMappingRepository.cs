/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Data;
using System;
using Dapper;
using System.Linq;
using PSW.ITT.Data.Entities;
using  PSW.ITT.Data.IRepositories;
using System.Collections.Generic;
using PSW.ITT.Data.DTO;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class AttributeValidationMappingRepository : Repository<AttributeValidationMapping>, IAttributeValidationMappingRepository
    {
		#region public constructors

        public AttributeValidationMappingRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[AttributeValidationMapping]";
			PrimaryKeyName = "ID";
        }

		#endregion

		#region Public methods
        public List<ProductCodeValidationList> GetAssociatedValidationList(List<int> sheetAttributeMappingIDs){
            var query = @"SELECT m.[Index] AS [Index], a.sheetAttributeMappingID AS SheetAttributeMappingID,
                        v.ID AS ValidationID, v.Value AS Validation, v.Description AS ValidationDescription,
                        m.TableName AS TableName, m.ColumnName AS ColumnName
                        from [dbo].[AttributeValidationMapping] a
                        JOIN [dbo].[Validation] v ON v.ID = a.ValidationID
                        JOIN [dbo].[SheetAttributeMapping] m ON a.SheetAttributeMappingID = m.ID
                        WHERE a.sheetAttributeMappingID in @SHEETATTRIBUTEMAPPINGIDS";
            return _connection.Query<ProductCodeValidationList>(
                    query,
                    param: new { SHEETATTRIBUTEMAPPINGIDS = sheetAttributeMappingIDs},
                    transaction: _transaction
                   ).ToList();
        }
		#endregion
    }
}
