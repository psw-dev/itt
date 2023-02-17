using System.Collections.Generic;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Data.IRepositories
{
    public interface IShrdCommonForLovRepository : IRepositoryTransaction 
    {
        List<(int,string)> GetLOV(string TableName, string ColumnName );
        List<string> GetDocumentLOV(string TableName, string ColumnName, string DocumentType, int AgencyID );
        List<string> GetList(string TableName, string ColumnName );
        List<string> GetListConsideringDate(string TableName, string ColumnName );
    }
}
