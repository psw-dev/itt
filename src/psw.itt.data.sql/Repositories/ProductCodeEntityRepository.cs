using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using psw.itt.data.Entities;
using psw.itt.data.IRepositories;

namespace psw.itt.data.sql.Repositories
{
    public class ProductCodeEntityRepository : Repository<ProductCodeEntity>, IProductCodeEntityRepository
    {
        #region public constructors

        public ProductCodeEntityRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ProductCode]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods
        public List<ProductCodeEntity> GetActiveProductCode()
        {
            try
            {
                var query = @"SELECT * FROM ProductCode WHERE EffectiveThruDt > GETDATE()";

                return _connection.Query<ProductCodeEntity>(query,
                transaction: _transaction
                ).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
