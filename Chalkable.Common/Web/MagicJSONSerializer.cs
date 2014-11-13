using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Script.Serialization;

namespace Chalkable.Common.Web
{
    public class MagicJsonSerializer
    {
        private const string JSON_INVALID_MAX_JSON_LENGTH = "Invalid max JSON length";
        private const string JSON_INVALID_RECURSION_LIMIT = "Invalid recursion limit";
        private const string JSON_MAX_JSON_LENGTH_EXCEEDED = "Max JSON length is exceeded";
        private const string JSON_DICTIONARY_TYPE_NOT_SUPPORTED = "Dictionary type is not supported";
        private const string JSON_INVALID_ENUM_TYPE = "Invalid enum type";
        public const int DEFAULT_MAX_JSON_LENGTH = 0x200000;
        public const string SERVER_TYPE_FIELD_NAME = "__type";

        public static readonly long DatetimeMinTimeTicks;
        private readonly JavaScriptTypeResolver typeResolver;
        private int maxDepth = 4;
        private int maxJsonLength;
        private readonly bool hideSensitive;
        
        static MagicJsonSerializer()
        {
            var time = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DatetimeMinTimeTicks = time.Ticks;
        }

        public MagicJsonSerializer(bool hideSensitive)
            : this(null, hideSensitive)
        {
        }

        public MagicJsonSerializer(JavaScriptTypeResolver resolver, bool hideSensitive)
        {
            typeResolver = resolver;
            MaxJsonLength = 64000000;
            this.hideSensitive = hideSensitive;
        }

        public int MaxJsonLength
        {
            get { return maxJsonLength; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(JSON_INVALID_MAX_JSON_LENGTH);
                }
                maxJsonLength = value;
            }
        }

        public int MaxDepth
        {
            get { return maxDepth; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(JSON_INVALID_RECURSION_LIMIT);
                }
                maxDepth = value;
            }
        }

        internal JavaScriptTypeResolver TypeResolver
        {
            get { return typeResolver; }
        }

        public string Serialize(object obj)
        {
            return Serialize(obj, SerializationFormat.Json);
        }

        public void Serialize(object obj, StringBuilder output)
        {
            Serialize(obj, output, SerializationFormat.Json);
        }

        private string Serialize(object obj, SerializationFormat serializationFormat)
        {
            var output = new StringBuilder();
            Serialize(obj, output, serializationFormat);
            return output.ToString();
        }

        internal void Serialize(object obj, StringBuilder output, SerializationFormat serializationFormat)
        {
            SerializeValue(obj, output, 0, null, serializationFormat, obj.GetType().ToString());
            if ((serializationFormat == SerializationFormat.Json) && (output.Length > MaxJsonLength))
            {
                throw new InvalidOperationException(JSON_MAX_JSON_LENGTH_EXCEEDED);
            }
        }

        private static void SerializeBoolean(bool o, StringBuilder sb)
        {
            if (o)
            {
                sb.Append("true");
            }
            else
            {
                sb.Append("false");
            }
        }

        private void SerializeCustomObject(object o, StringBuilder sb, int depth, Hashtable objectsInUse,
                                           SerializationFormat serializationFormat)
        {
            if (++depth <= maxDepth)
            {
                bool flag = true;
                Type type = o.GetType();
                sb.Append('{');
                if (TypeResolver != null)
                {
                    string str = TypeResolver.ResolveTypeId(type);
                    if (str != null)
                    {
                        SerializeString("__type", sb);
                        sb.Append(':');
                        SerializeValue(str, sb, depth, objectsInUse, serializationFormat, "__type");
                        flag = false;
                    }
                }
                if (depth < maxDepth)
                {
                    var properties = type.GetProperties((BindingFlags.GetProperty & BindingFlags.SetProperty) | BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo info2 in properties)
                    {
                        if (hideSensitive && info2.IsDefined(typeof(SensitiveData), true)) continue;
                        if (info2.IsDefined(typeof(ScriptIgnoreAttribute), true)) continue;
                        MethodInfo getMethod = info2.GetGetMethod();
                        if ((getMethod != null) && (getMethod.GetParameters().Length <= 0))
                        {
                            var value = getMethod.Invoke(o, null);
                            if (objectsInUse == null)
                            {
                                objectsInUse = new Hashtable(new ReferenceComparer());
                            }
                            else if (value != null && !((IList)Constants.BuiltInTypes).Contains(value.GetType()) && objectsInUse.ContainsKey(value))
                            {
                                continue;
                            }
                            if (value != null && !((IList)Constants.BuiltInTypes).Contains(value.GetType()))
                            {
                                objectsInUse.Add(value, null);
                            }
                            if (!flag)
                            {
                                sb.Append(',');
                            }
                            SerializeString(info2.Name.ToLower(), sb);
                            sb.Append(':');
                            SerializeValue(value, sb, depth, objectsInUse, serializationFormat, info2.Name.ToLower());
                            flag = false;
                        }
                    }
                }
                sb.Append('}');
            }
        }

        private static void SerializeDateTime(DateTime datetime, StringBuilder sb, SerializationFormat serializationFormat, string propertyName)
        {
            if (serializationFormat == SerializationFormat.Json)
            {
                sb.Append('"');
                if (propertyName.EndsWith("time"))
                {
                    sb.Append(String.Format("{0:t}", datetime));
                }
                else if (propertyName.EndsWith("date"))
                {
                    sb.Append(datetime.ToString(Constants.DATE_FORMAT));
                }
                else
                {
                    sb.Append(String.Format("{0:M/d/yyyy hh:mm:ss tt}", datetime));
                }
                sb.Append('"');
            }
            else
            {
                sb.Append("new Date(");
                sb.Append(((datetime.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 0x2710L));
                sb.Append(")");
            }
        }

        private void SerializeDictionary(IDictionary o, StringBuilder sb, int depth, Hashtable objectsInUse,
                                         SerializationFormat serializationFormat)
        {
            sb.Append('{');
            bool flag = true;
            bool flag2 = false;
            if (o.Contains("__type"))
            {
                flag = false;
                flag2 = true;
                SerializeDictionaryKeyValue("__type", o["__type"], sb, depth, objectsInUse, serializationFormat);
            }
            foreach (DictionaryEntry entry in o)
            {
                var key = entry.Key as string;
                if (key == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,JSON_DICTIONARY_TYPE_NOT_SUPPORTED,
                                                              new object[] { o.GetType().FullName }));
                }
                if (flag2 && string.Equals(key, "__type", StringComparison.Ordinal))
                {
                    flag2 = false;
                }
                else
                {
                    if (!flag)
                    {
                        sb.Append(',');
                    }
                    SerializeDictionaryKeyValue(key, entry.Value, sb, depth, objectsInUse, serializationFormat);

                    flag = false;
                }
            }
            sb.Append('}');
        }

        private void SerializeDictionaryKeyValue(string key, object value, StringBuilder sb, int depth,
                                                 Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            SerializeString(key.ToLower(), sb);
            sb.Append(':');
            SerializeValue(value, sb, depth, objectsInUse, serializationFormat, key.ToLower());
        }

        private void SerializeEnumerable(IEnumerable enumerable, StringBuilder sb, int depth, Hashtable objectsInUse,
                                         SerializationFormat serializationFormat)
        {
            sb.Append('[');
            bool flag = true;
            foreach (object obj2 in enumerable)
            {
                if (!flag)
                {
                    sb.Append(',');
                }
                SerializeValue(obj2, sb, depth, objectsInUse, serializationFormat, "");
                flag = false;
            }
            sb.Append(']');
        }

        private static void SerializeGuid(Guid guid, StringBuilder sb)
        {
            sb.Append("\"").Append(guid.ToString()).Append("\"");
        }

        internal static string SerializeInternal(object o)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(o);
        }

        private static void SerializeString(string input, StringBuilder sb)
        {
            sb.Append('"');
            sb.Append(JavaScriptString.QuoteString(input));
            sb.Append('"');
        }

        private static void SerializeUri(Uri uri, StringBuilder sb)
        {
            sb.Append("\"").Append(uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped)).Append("\"");
        }

        private void SerializeValue(object o, StringBuilder sb, int depth, Hashtable objectsInUse,
                                    SerializationFormat serializationFormat, string propertyName)
        {
            SerializeValueInternal(o, sb, depth, objectsInUse, serializationFormat, propertyName);
        }

        private void SerializeValueInternal(object o, StringBuilder sb, int depth, Hashtable objectsInUse,
                                            SerializationFormat serializationFormat, string propertyName)
        {
            if ((o == null) || DBNull.Value.Equals(o))
            {
                sb.Append("null");
            }
            else
            {
                var input = o as string;
                if (input != null)
                {
                    SerializeString(input, sb);
                }
                else if (o is char)
                {
                    if (((char)o) == '\0')
                    {
                        sb.Append("null");
                    }
                    else
                    {
                        SerializeString(o.ToString(), sb);
                    }
                }
                else if (o is bool)
                {
                    SerializeBoolean((bool)o, sb);
                }
                else if (o is DateTime)
                {
                    SerializeDateTime((DateTime)o, sb, serializationFormat, propertyName);
                }
                else if (o is Guid)
                {
                    SerializeGuid((Guid)o, sb);
                }
                else
                {
                    var uri = o as Uri;
                    if (uri != null)
                    {
                        SerializeUri(uri, sb);
                    }
                    else if (o is double)
                    {
                        sb.Append(((double)o).ToString("r", CultureInfo.InvariantCulture));
                    }
                    else if (o is float)
                    {
                        sb.Append(((float)o).ToString("r", CultureInfo.InvariantCulture));
                    }
                    else if (o.GetType().IsPrimitive || (o is decimal))
                    {
                        var convertible = o as IConvertible;
                        if (convertible != null)
                        {
                            sb.Append(convertible.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            sb.Append(o);
                        }
                    }
                    else
                    {
                        Type enumType = o.GetType();
                        if (enumType.IsEnum)
                        {
                            Type underlyingType = Enum.GetUnderlyingType(enumType);
                            if ((underlyingType == typeof(long)) || (underlyingType == typeof(ulong)))
                            {
                                throw new InvalidOperationException(JSON_INVALID_ENUM_TYPE);
                            }
                            sb.Append(((Enum)o).ToString("D"));
                        }
                        else
                        {
                            try
                            {
                                var dictionary = o as IDictionary;
                                if (dictionary != null)
                                {
                                    SerializeDictionary(dictionary, sb, depth, objectsInUse, serializationFormat);
                                }
                                else
                                {
                                    var enumerable = o as IEnumerable;
                                    if (enumerable != null)
                                    {
                                        SerializeEnumerable(enumerable, sb, depth, objectsInUse, serializationFormat);
                                    }
                                    else
                                    {
                                        SerializeCustomObject(o, sb, depth, objectsInUse, serializationFormat);
                                    }
                                }
                            }
                            finally
                            {
                                if (objectsInUse != null)
                                {
                                    objectsInUse.Remove(o);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Nested Types

        #region Nested type: ReferenceComparer

        private class ReferenceComparer : IEqualityComparer
        {
            // Methods

            #region IEqualityComparer Members

            bool IEqualityComparer.Equals(object x, object y)
            {
                return (x == y);
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }

            #endregion
        }

        #endregion

        #region Nested type: SerializationFormat

        internal enum SerializationFormat
        {
            Json,
            JavaScript
        }

        #endregion
    }
}