using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;
using SqlKata;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ShrdCommonForLovRepository : SHRDRepository, IShrdCommonForLovRepository
    {
        #region public constructors

        public ShrdCommonForLovRepository(IDbConnection context) : base(context)
        {
          
        }

        #endregion

        #region Public methods
 public List<(int,string)> GetLOV(string TableName, string ColumnName )
        {
            var query = @"SELECT id,"+ColumnName+"  FROM "+ TableName + " WHERE IsActive = 1";
             var returnValue = _connection.Query<(int,string)>(
                    query 
                   ).ToList();
                   return returnValue;
        }
        
 public List<string> GetList(string TableName, string ColumnName )
        {
            var query = @"SELECT "+ColumnName+"  FROM "+ TableName + " WHERE IsActive = 1";
             var returnValue = _connection.Query<string>(
                    query 
                   ).ToList();
                   return returnValue;
        }

    
    public List<string> GetDocumentLOV(string TableName, string ColumnName, string DocumentType, int AgencyID)
    {
        var table = TableName. Split(',').ToList();
        
           var join = table[0] + " a join "+table[1]+ " b on a.DocumentTypeCode = b.Code ";
            var query = @"SELECT "+ColumnName+"  FROM "+ join + " WHERE a.IsActive = 1 and a.UserAgencyID ="+ AgencyID+" and a.DocumentGroupCode  = '"+ DocumentType+"'";
             var returnValue = _connection.Query<string>(
                    query 
                   ).ToList();
                   return returnValue;
        }
    }     
        #endregion
}