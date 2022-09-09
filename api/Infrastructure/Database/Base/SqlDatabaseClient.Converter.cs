using Npgsql;

namespace SpoRE.Infrastructure.Base;

public static partial class SqlDatabaseClient
{
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

    private static T GetObject<T>(this Dictionary<string, object> dict)
    {
        var type = typeof(T);
        var obj = Activator.CreateInstance(type);

        foreach (var kv in dict)
        {
            var prop = type.GetProperty(kv.Key);
            if (prop == null) continue;

            object value = kv.Value;
            if (value is Dictionary<string, object>)
            {
                value = GetObject<T>((Dictionary<string, object>)value);
            }

            prop.SetValue(obj, value, null);
        }
        return (T)obj;
    }
}