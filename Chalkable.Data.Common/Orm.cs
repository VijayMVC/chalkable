using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Chalkable.Common;

namespace Chalkable.Data.Common
{
    public static class Orm
    {
        private static bool IsDbField(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<DataEntityAttr>() == null
                && propertyInfo.GetCustomAttribute<NotDbFieldAttr>() == null;
        }

        public static List<string> Fields(Type t)
        {
            var res = new List<string>();
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in props)
                if (propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    if (IsDbField(propertyInfo))
                        res.Add(propertyInfo.Name);
                }
            return res;
        }

        public static List<string> Fields<T>()
        {
            var t = typeof (T);
            return Fields(t);
        }

        public static DbQuery SimpleInsert<T>(T obj)
        {
            var res = new DbQuery();
            res.Parameters = new Dictionary<string, object>();
            var b = new StringBuilder();
            var t = typeof (T);
            var fields = Fields(t);
            b.Append("Insert into [").Append(t.Name).Append("] (");
            b.Append(fields.Select(x=>"[" + x + "]").JoinString(",")).Append(") values (");
            b.Append(fields.Select(x => "@" + x).JoinString(",")).Append(")");
            res.Sql = b.ToString();
            foreach (var field in fields)
            {
                res.Parameters.Add("@" + field, t.GetProperty(field).GetValue(obj));
            }
            return res;
        }

        public static DbQuery SimpleUpdate<T>(T obj)
        {
            var res = new DbQuery();
            res.Parameters = new Dictionary<string, object>();
            var b = new StringBuilder();
            var t = typeof(T);
            var fields = Fields(t);
            
            b.Append("Update [").Append(t.Name).Append("] set ");
            b.Append(fields.Select(x => "[" + x + "]=@" + x).JoinString(",")).Append(" where [Id] = @Id");
            res.Sql = b.ToString();
            foreach (var field in fields)
            {
                res.Parameters.Add("@" + field, t.GetProperty(field).GetValue(obj));
            }
            return res;
        }

        public static DbQuery SimpleDelete<T>(T obj)
        {
            var res = new DbQuery();
            res.Parameters = new Dictionary<string, object>();
            var b = new StringBuilder();
            var t = typeof(T);
            b.Append("Delete from [").Append(t.Name).Append("] where Id = @Id");
            res.Parameters.Add("@Id", t.GetProperty("Id").GetValue(obj));
            res.Sql = b.ToString();
            return res;
        }
    }

    public class DbQuery
    {
        public string Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; } 
    }
}
