using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace Chalkable.Data.Common
{
    public static class SqlTools
    {
        public static byte ReadByte(DbDataReader reader, string field)
        {
            return reader.GetByte(reader.GetOrdinal(field));
        }

        public static short ReadInt16(DbDataReader reader, string field)
        {
            return reader.GetInt16(reader.GetOrdinal(field));
        }
        
        public static int ReadInt32(DbDataReader reader, string field)
        {
            return reader.GetInt32(reader.GetOrdinal(field));
        }

        public static long ReadInt64(DbDataReader reader, string field)
        {
            return reader.GetInt64(reader.GetOrdinal(field));
        }

        public static long? ReadInt64Null(DbDataReader reader, string field)
        {
            if (reader.IsDBNull(reader.GetOrdinal(field)))
                return null;
            return ReadInt64(reader, field);
        }

        public static string ReadStringNull(DbDataReader reader, string field)
        {
            if (reader.IsDBNull(reader.GetOrdinal(field)))
                return null;
            return reader.GetString(reader.GetOrdinal(field));
        }

        public static DateTime ReadDateTime(DbDataReader reader, string field)
        {
            return reader.GetDateTime(reader.GetOrdinal(field));
        }

        public static DateTime? ReadDateTimeNull(DbDataReader reader, string field)
        {
            if (reader.IsDBNull(reader.GetOrdinal(field)))
                return null;
            return reader.GetDateTime(reader.GetOrdinal(field));
        }

        public static int? ReadInt32Null(DbDataReader reader, string field)
        {
            if (reader.IsDBNull(reader.GetOrdinal(field)))
                return null;
            return reader.GetInt32(reader.GetOrdinal(field));
        }

        public static bool ReadBool(DbDataReader reader, string field)
        {
            return reader.GetBoolean(reader.GetOrdinal(field));
        }

        public static bool? ReadBoolNull(DbDataReader reader, string field)
        {
            int ordinal = reader.GetOrdinal(field);
            if (reader.IsDBNull(ordinal))
                return null;
            return reader.GetBoolean(ordinal);
        }

        public static double ReadDouble(DbDataReader reader, string field)
        {
            return reader.GetDouble(reader.GetOrdinal(field));
        }

        public static decimal ReadDecimal(DbDataReader reader, string field)
        {
            return reader.GetDecimal(reader.GetOrdinal(field));
        }

        public static Guid ReadGuid(DbDataReader reader, string field)
        {
            return reader.GetGuid(reader.GetOrdinal(field));
        }

        public static object ReadGuidNull(DbDataReader reader, string field)
        {
            if (reader.IsDBNull(reader.GetOrdinal(field)))
                return null;
            return ReadGuid(reader, field);
        }

        public static decimal? ReadDecimalNull(DbDataReader reader, string field)
        {
            if (reader.IsDBNull(reader.GetOrdinal(field)))
                return null;
            return ReadDecimal(reader, field);
        }

        public static bool ColumnExists(this DbDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == columnName)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNullableEnum(this Type t)
        {
            return t.IsGenericType &&
                   t.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   t.GetGenericArguments()[0].IsEnum;
        }

        private static object Read(DbDataReader reader, string field, Type type)
        {
            if (!reader.ColumnExists(field))
                return null;
            if (type == typeof(Guid))
                return ReadGuid(reader, field);
            if (type == typeof (Guid?))
                return ReadGuidNull(reader, field);
            if (type == typeof (byte))
                return ReadByte(reader, field);
            if (type == typeof (short))
                return ReadInt16(reader, field);
            if (type == typeof (int))
                return ReadInt32(reader, field);
            if (type == typeof(int?))
                return ReadInt32Null(reader, field);
            if (type == typeof(bool))
                return ReadBool(reader, field);
            if (type == typeof(bool?))
                return ReadBoolNull(reader, field);

            if (type == typeof(DateTime))
                return ReadDateTime(reader, field);
            if (type == typeof(DateTime?))
                return ReadDateTimeNull(reader, field);

            if (type == typeof(long))
                return ReadInt64(reader, field);
            if (type == typeof(long?))
                return ReadInt64Null(reader, field);

            if (type == typeof(decimal))
                return ReadDecimal(reader, field);
            if (type == typeof(decimal?))
                return ReadDecimalNull(reader, field);

            if (type == typeof(string))
                return ReadStringNull(reader, field);

            if (type == typeof(double))
                return ReadDouble(reader, field);


            if (type.IsNullableEnum())
                return ReadInt32Null(reader, field);
            if (type.IsEnum)
                return ReadInt32(reader, field);
            
            if (type.IsNullableEnum())
                return ReadInt32Null(reader, field);
            
            return null;
        }


        public static object Read(this DbDataReader reader, Type t, bool fullFieldNames = false)
        {
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var res = Activator.CreateInstance(t);
            var abstractTypeName = GetBaseModelName(t);
            foreach (var propertyInfo in props)
                if (propertyInfo.CanWrite)
                {
                    object value;
                    if (propertyInfo.GetCustomAttribute<DataEntityAttr>() != null)
                        value = reader.Read(propertyInfo.PropertyType, fullFieldNames);
                    else
                    {
                        var fieldName = propertyInfo.Name;
                        if (fullFieldNames)
                            fieldName = string.Format("{0}_{1}", abstractTypeName, fieldName);
                        value = Read(reader, fieldName, propertyInfo.PropertyType);    
                    }
                    if(value != null)    
                        propertyInfo.SetValue(res, value);
                }
            return res;
        }

        private static string GetBaseModelName(Type t)
        {
            if (t.BaseType != typeof (Object))
                return GetBaseModelName(t.BaseType);
            return t.Name;
        }

        public static T Read<T>(this DbDataReader reader, bool complexResultSet = false) where T : new()
        {
            var t = typeof (T);
            return (T)reader.Read(t, complexResultSet);
        }

        public static T ReadOrNull<T>(this DbDataReader reader, bool complexResultSet = false) where T : new()
        {
            if (reader.Read())
            {
                var t = typeof(T);
                return (T)reader.Read(t, complexResultSet);    
            }
            return default(T);
        }

        public static IList<T> ReadList<T>(this DbDataReader reader, bool complexResultSet = false) where T : new()
        {
            var res = new List<T>();
            while (reader.Read())
            {
                var o = reader.Read<T>(complexResultSet);
                res.Add(o);
            }
            return res;
        }
    }
}
