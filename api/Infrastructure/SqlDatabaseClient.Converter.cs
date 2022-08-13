using Npgsql;

namespace SpoRE.Infrastructure.SqlDatabaseClient
{
    public static partial class SqlDatabaseClient
    {
        private static List<Object> ConvertResult(NpgsqlDataReader dataReader)
        {
            List<Object> result = new List<Object>();
            while (dataReader.Read())
            {
                var values = new Dictionary<string, object>();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    values.Add(dataReader.GetName(i), dataReader[i]);
                }
                result.Add(values);
            }
            return result;
        }

        private static List<T> ConvertResult<T>(NpgsqlDataReader dataReader)
        {
            List<T> result = new List<T>();
            while (dataReader.Read())
            {
                var values = new Dictionary<string, object>();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    values.Add(dataReader.GetName(i), dataReader[i]);
                }
                result.Add(GetObject<T>(values));
            }
            return result;
        }

        private static Object GetObject(this Dictionary<string, object> dict, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop == null) continue;

                object value = kv.Value;
                if (value is Dictionary<string, object>)
                {
                    value = GetObject((Dictionary<string, object>)value, prop.PropertyType);
                }

                prop.SetValue(obj, value, null);
            }
            return obj;
        }

        private static T GetObject<T>(this Dictionary<string, object> dict)
        {
            return (T)GetObject(dict, typeof(T));
        }
    }
}