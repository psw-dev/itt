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
    public abstract class Repository<T> : IRepository<T> where T : Entity
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
        #region Protected Constructors

        protected Repository(IDbConnection connection)
        {
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
            _connection = connection;
            _sqlCompiler = new SqlServerCompiler();
        }

        #endregion

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

        public IEnumerable<T> All()
        {
            //TODO: Implement generic query all method
            return Where("1=1");
        }
        public void Delete(int id)
        {

            _connection.Query<T>(
                "DELETE FROM @TableName WHERE @PrimaryKeyName = @Id",
                new { TableName = TableName, PrimaryKeyName = PrimaryKeyName, Id = id },
                _transaction
                );
        }

        public void Delete(T entity)
        {
            var columns = entity.GetColumns();
            var sqlQuery = string.Format("DELETE {0} WHERE {2} = '{1}';", TableName, columns[PrimaryKeyName], PrimaryKeyName);

            _connection.Execute(
                sqlQuery, transaction: _transaction
            );
        }
        public virtual T Get(string id)
        {
            return _connection.Query<T>(string.Format("SELECT TOP 1 * FROM {0} WHERE [{2}] = '{1}'", TableName, id, PrimaryKeyName),
                                        transaction: _transaction).FirstOrDefault();
        }
        public virtual T Get(long id)
        {
            return _connection.Query<T>(string.Format("SELECT TOP 1 * FROM {0} WHERE [{2}] = '{1}'", TableName, id, PrimaryKeyName),
                                        transaction: _transaction).FirstOrDefault();
        }
        // public T Find(int id)
        // {

        //     //TODO: Implement generic 
        //     //TODO: Implement generic 
        //     return _connection.Query<T>(
        //         "SELECT top 1 * FROM @TableName WHERE @PrimaryKeyName = @Id",
        //         param: new { TableName=this.TableName,PrimaryKeyName=this.PrimaryKeyName, Id = id },
        //         transaction: _transaction
        //         ).FirstOrDefault();
        // }

        public T Find(int id)
        {
            return _connection.Query<T>(
                $"SELECT top 1 * FROM {TableName} where {PrimaryKeyName} = {id}",
                new { TableName = TableName, PrimaryKeyName = PrimaryKeyName, Id = id },
                _transaction
                ).FirstOrDefault();
        }

        public int Update(T entity)
        {
            var values = new StringBuilder();
            var columns = entity.GetColumns();
            foreach (var item in columns.Where(c => c.Key != entity.PrimaryKeyName))
            {

                values.Append(string.Format(item.Value == null ? "{0}={1}," : "{0}='{1}',", item.Key, item.Value == null ? "null" : item.Value.ToString()));
            }

            // Need to Remove last comma
            var colValues = values.ToString().TrimEnd(',');


            return _connection.Query<T>(
                $"Update {TableName} Set {colValues} WHERE {PrimaryKeyName} = {columns[PrimaryKeyName]}",
                new { TableName = TableName, Values = colValues, PrimaryKeyName = PrimaryKeyName, Id = columns[PrimaryKeyName] },
                _transaction
                ).Count();

        }

        public int Update(T entity, List<string> columnNames)
        {

            var values = new StringBuilder();
            var columns = entity.GetColumns();
            foreach (var item in columns.Where(c => c.Key != entity.PrimaryKeyName))
            {
                if (columnNames.Contains(item.Key))
                    values.Append(string.Format(item.Value == null ? "{0}={1}," : "{0}='{1}',", item.Key, item.Value == null ? "null" : item.Value.ToString()));
            }

            // Need to Remove last comma
            var colValues = values.ToString().TrimEnd(',');

            return _connection.Query<T>(
                $"Update {TableName} Set {colValues} WHERE {PrimaryKeyName} = {columns[PrimaryKeyName]}",
                new { TableName = TableName, Values = colValues, PrimaryKeyName = PrimaryKeyName, Id = columns[PrimaryKeyName] },
                _transaction
            ).Count();
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

        public string WhereBuilder(object condition)
        {
            var whereBuilder = new StringBuilder();
            var objectType = condition.GetType();
            var first = true;
            foreach (var property in objectType.GetProperties())
            {
                whereBuilder.AppendFormat(" {2} {0} = '{1}'", property.Name, property.GetValue(condition).ToString().Replace("'", "''"), first ? "" : "AND");
                first = false;
            }
            return whereBuilder.ToString();
        }
        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }
    }

}
