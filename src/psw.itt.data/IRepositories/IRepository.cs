using System.Collections.Generic;

namespace psw.itt.data.IRepositories
{
    public interface IRepository<T> : IRepositoryTransaction
    {
        int Add(T entity);
        IEnumerable<T> All();
        void Delete(int id);
        void Delete(T entity);
        T Find(int id);
        // T FindByName(string name);
        int Update(T entity);
        int Update(T entity, List<string> columnNames);
        IEnumerable<T> Where(string condition);

        List<T> Where(object condition);


        string WhereBuilder(object condition);
        T Get(string id);

    }
}