using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;
using SqlKata.Compilers;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

// using SqlKata;
// using SqlKata.Compilers;

namespace PSW.ITT.Data.Sql.Repositories
{
    public abstract class SHRDRepository :ISHRDRepository
    {
        #region Protected properties
        protected SqlServerCompiler _sqlCompiler;

        #endregion

        #region Public properties

        #endregion
        #region Protected Fields
        protected IDbConnection _connection;// { get { return _transaction.Connection; } }
        #endregion
        #region Protected Constructors

        protected SHRDRepository(IDbConnection connection)
        {
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
            _connection = connection;
        }

        #endregion

        public virtual List<(int,string)> Get(string ColumnName, string TableName)
        {
            return _connection.Query<(int,string)>(string.Format("SELECT TOP 1 * FROM {0} WHERE [{2}] = '{1}'", TableName)).ToList();
        }
      
        public List<(int,string)> Where(string tableName, string condition)
        {
            return _connection.Query<(int,string)>(
                $"SELECT * from  {tableName} where {condition}",
                new { TableName = tableName, Condition = condition }
            ).ToList<(int,string)>();
        }

    

        // public List<(int,string)> Where(object propertyValues)
        // {
        //     const string query = "SELECT * FROM {0} WHERE {1}";

        //     var whereBulder = new StringBuilder();
        //     var objectType = propertyValues.GetType();
        //     var first = true;
        //     foreach (var property in objectType.GetProperties())
        //     {
        //         whereBulder.AppendFormat("{2} [{0}] = '{1}'", property.Name, property.GetValue(propertyValues).ToString().Replace("'", "''"), first ? "" : "AND");
        //         first = false;
        //     }
        //     return _connection.Query<T>(string.Format(query, TableName, whereBulder), transaction: _transaction).ToList();
        // }

        // public string WhereBuilder(object condition)
        // {
        //     var whereBuilder = new StringBuilder();
        //     var objectType = condition.GetType();
        //     var first = true;
        //     foreach (var property in objectType.GetProperties())
        //     {
        //         whereBuilder.AppendFormat(" {2} {0} = '{1}'", property.Name, property.GetValue(condition).ToString().Replace("'", "''"), first ? "" : "AND");
        //         first = false;
        //     }
        //     return whereBuilder.ToString();
        // }
       
    }

}
