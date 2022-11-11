using System.Collections.Generic;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Data.IRepositories
{
    public interface IShrdCommonForLovRepository : ISHRDRepository
    {
        List<(int,string)> GetLOV(string TableName, string ColumnName );
    }
}
