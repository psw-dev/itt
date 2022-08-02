using System.Collections.Generic;

namespace psw.itt.data.Entities
{
    public abstract class Entity
    {
        public string TableName { get; protected set; }
        public string PrimaryKeyName { get; protected set; }
        public object PrimaryKey { get; set; }

        public virtual Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object>();
        }
    }
}
