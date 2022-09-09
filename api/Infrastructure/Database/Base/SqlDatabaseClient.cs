using Npgsql;

namespace SpoRE.Infrastructure.Base;

public static partial class SqlDatabaseClient
{
    public static async Task<List<T>> Get<T>(string query, Dictionary<string, object> parameters)
    {
        var SqlConnectionString = Secrets.SqlConnectionString;
        using var con = new NpgsqlConnection(SqlConnectionString);
        con.Open();
        using var cmd = new NpgsqlCommand(query, con);
        foreach (var (name, value) in parameters)
        {
            cmd.Parameters.AddWithValue(name, value);
        }

        try
        {
            var dataReader = await cmd.ExecuteReaderAsync();
            return ConvertResult<T>(dataReader);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("SQL EXCEPTION: \n" + ex);
            return new();
        }
    }

    public static async Task<T> GetSingle<T>(string query, Dictionary<string, object> parameters)
    {
        var output = await Get<T>(query, parameters);
        return output.Count == 1 ? output.Single() : default;
    }
}