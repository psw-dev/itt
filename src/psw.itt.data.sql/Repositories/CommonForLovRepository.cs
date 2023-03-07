using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;
using SqlKata;
using SqlKata.Compilers;
using PSW.Lib.Logs;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class CommonForLovRepository : ICommonForLovRepository
    {
        #region Protected properties
        protected string TableName { get; set; }
        protected string PrimaryKeyName { get; set; }
        protected SqlServerCompiler _sqlCompiler;

        #endregion

        #region Public properties

        public Entity Entity { get; set; }

        #endregion
        #region Protected Fields
        protected IDbTransaction _transaction;//{ get; private set; }
        protected IDbConnection _connection;// { get { return _transaction.Connection; } }
        #endregion

        #region public constructors
        
        public CommonForLovRepository(IDbConnection connection)
        {
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
            _connection = connection;
            _sqlCompiler = new SqlServerCompiler();
        }
        // public ShrdCommonForLovRepository(IDbConnection context) : base(context)
        // {
          
        // }

        #endregion

        #region Public methods
    public List<(int,string)> GetLOV(string TableName, string ColumnName )
        {
            Log.Information("[{0}] ConnectionString", _connection.ConnectionString);
            var query = @"SELECT id,"+ColumnName+"  FROM "+ TableName  ;
             var returnValue = _connection.Query<(int,string)>(
                    query 
                   ).ToList();
                   return returnValue;
        }
        
    public List<string> GetList(string TableName, string ColumnName )
        {
            Log.Information("[{0}] ConnectionString", _connection.ConnectionString);
            var query = @"SELECT "+ColumnName+"  FROM "+ TableName ;
             var returnValue = _connection.Query<string>(
                    query 
                   ).ToList();
                   return returnValue;
        }
    public List<string> GetListConsideringDate(string TableName, string ColumnName )
    {
        Log.Information("[{0}] ConnectionString", _connection.ConnectionString);
        var query = @"SELECT "+ColumnName+"  FROM "+ TableName + " WHERE Effective_Date <=GetDate() AND End_Date>=GetDate() ;";
            var returnValue = _connection.Query<string>(
                query 
                ).ToList();
                return returnValue;
    }

    
    public List<string> GetDocumentLOV(string TableName, string ColumnName, string DocumentType, int AgencyID)
    {
        var table = TableName. Split(',').ToList();
        
            Log.Information("[{0}] ConnectionString", _connection.ConnectionString);
           var join = table[0] + " a join "+table[1]+ " b on a.DocumentTypeCode = b.Code ";
            var query = @"SELECT "+ColumnName+"  FROM "+ join + " WHERE a.IsActive = 1 and a.UserAgencyID ="+ AgencyID+" and a.DocumentGroupCode  = '"+ DocumentType+"'";
             var returnValue = _connection.Query<string>(
                    query 
                   ).ToList();
                   return returnValue;
        }
     public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }
        #endregion
    }     
       
}
