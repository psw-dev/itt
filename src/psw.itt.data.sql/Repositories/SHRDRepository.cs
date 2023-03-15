using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;
using SqlKata.Compilers;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System;

// using SqlKata;
// using SqlKata.Compilers;

namespace PSW.ITT.Data.Sql.Repositories
{
    public abstract class SHRDRepository<T> :ISHRDRepository<T> where T : Entity
    {
        #region Protected properties
        protected SqlServerCompiler _sqlCompiler;
        protected IDbTransaction _transaction;//{ get; private set; }

        #endregion

        #region Public properties
        
        protected string TableName { get; set; }
        protected string PrimaryKeyName { get; set; }

        #endregion
        #region Protected Fields
        protected IDbConnection _connection;// { get { return _transaction.Connection; } }
        #endregion
        #region Protected Constructors

        #region Public properties

        public Entity Entity { get; set; }

        #endregion

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

        public int Add(T _entity)
        {
            var Entity = _entity;
            var query = "INSERT INTO {0} ({1}) VALUES({2}); SELECT SCOPE_IDENTITY()";

            var cols = new StringBuilder();
            var values = new StringBuilder();


            var columns = Entity.GetColumns();
            foreach (var item in columns.Where(c => c.Key != Entity.PrimaryKeyName))
            {
                cols.Append("[" + item.Key + "],");
                if (item.Value == null)
                    values.Append("NULL,");
                else
                    values.AppendFormat("'{0}',", item.Value.ToString().Replace("'", "''"));
            }

            query = string.Format(query, Entity.TableName, cols.ToString().TrimEnd(','), values.ToString().TrimEnd(',')) + ";";

            var result = _connection.ExecuteScalar<int>(query, transaction: _transaction);
            // return Connection.Query<T>(
            //     query,transaction: _transaction
            //     );
            return result;
        }

        public IEnumerable<T> Get()
        {
            return (IEnumerable<T>)_connection.Query<T>(string.Format("SELECT * FROM {0}", TableName),
                                                        transaction: _transaction);
        }

         public IEnumerable<T> Where(string condition)
        {
            return _connection.Query<T>(
                $"SELECT * from  {TableName} where {condition}",
                new { TableName = TableName, Condition = condition },

                _transaction
            ).ToList<T>();
        }

        // public IEnumerable<T> Where(object condition){            
        //     string whereQuery = WhereBuilder(condition);
        //     return this.Where(whereQuery);
        // }

        public List<T> Where(object propertyValues)
        {
            const string query = "SELECT * FROM {0} WHERE {1}";

            var whereBulder = new StringBuilder();
            var objectType = propertyValues.GetType();
            var first = true;
            foreach (var property in objectType.GetProperties())
            {
                whereBulder.AppendFormat("{2} [{0}] = '{1}'", property.Name, property.GetValue(propertyValues).ToString().Replace("'", "''"), first ? "" : "AND");
                first = false;
            }
            return _connection.Query<T>(string.Format(query, TableName, whereBulder), transaction: _transaction).ToList();
        }
       public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }
    }

}
