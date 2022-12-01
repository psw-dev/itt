using System.Collections.Generic;

namespace PSW.ITT.Data.IRepositories
{
    public interface ISHRDRepository
    {
        List<(int,string)> Where(string ColumnName, string TableName);

        List<(int,string)> Get(string ColumnName, string TableName);

    }
}