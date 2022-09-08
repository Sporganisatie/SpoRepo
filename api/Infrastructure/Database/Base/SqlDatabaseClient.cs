using Npgsql;

namespace SpoRE.Infrastructure.Base;

public static partial class SqlDatabaseClient
{
    public static async Task<List<T>> Get<T>(string query)
    {
        var SqlConnectionString = Secrets.SqlConnectionString;
        using var con = new NpgsqlConnection(SqlConnectionString);
        con.Open();
        using var cmd = new NpgsqlCommand(query, con);

        try
        {
            var dataReader = await cmd.ExecuteReaderAsync();
            return ConvertResult<T>(dataReader);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine(ex);
            return new();
        }
    }

    public static async Task<T> GetSingle<T>(string query) //TODO result? class eromheen voor notfound
    {
        var output = await Get<T>(query);
        return output.Count == 1 ? output.Single() : default; // TODO some error
    }

    public static async Task<List<object>> ProcessFreeQuery(string query) // deze bestaat alleen voor de admin page met open queries
    {// kan ik wss fuseren met Get
        var SqlConnectionString = Secrets.SqlConnectionString;
        using var con = new NpgsqlConnection(SqlConnectionString);
        con.Open();
        using var cmd = new NpgsqlCommand(query, con);

        try
        {
            var dataReader = await cmd.ExecuteReaderAsync();
            return ConvertResult(dataReader);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine(ex);
            return new();
        }
    }
}