using System.Collections.Generic;
using psw.itt.data.Entities;

namespace psw.itt.data.IRepositories
{
    public interface IProductCodeEntityRepository : IRepository<ProductCodeEntity>
    {
        List<ProductCodeEntity> GetActiveProductCode();
    }
}
