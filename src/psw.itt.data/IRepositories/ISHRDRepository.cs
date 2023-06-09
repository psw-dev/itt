﻿using System.Collections.Generic;

namespace PSW.ITT.Data.IRepositories
{
    public interface ISHRDRepository<T> : IRepositoryTransaction
    {
        List<(int,string)> Where(string ColumnName, string TableName);

        List<(int,string)> Get(string ColumnName, string TableName);

         int Add(T entity);
        // IEnumerable<T> All();
        // void Delete(int id);
        // void Delete(T entity);
        // T Find(int id);
        // // T FindByName(string name);
        // int Update(T entity);
        // int Update(T entity, List<string> columnNames);
        IEnumerable<T> Where(string condition);

        List<T> Where(object condition);


        // string WhereBuilder(object condition);
        // T Get(string id);
        // T Get(long id);
        IEnumerable<T> Get();
    }
}