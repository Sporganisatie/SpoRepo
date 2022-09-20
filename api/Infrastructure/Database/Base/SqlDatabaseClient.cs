using Microsoft.Extensions.Options;
using Npgsql;
using SpoRE.Models;
using SpoRE.Models.Settings;

namespace SpoRE.Infrastructure.Base;

public partial class SqlDatabaseClient
{
    private AppSettings _configuration;
    public SqlDatabaseClient(IOptions<AppSettings> configuration)
    {
        _configuration = configuration.Value;
    }

    public async Task<Result<List<T>>> Get<T>(string query, Dictionary<string, object> parameters)
    {
        using var con = new NpgsqlConnection(_configuration.MyDbConnection);
        con.Open();
        using var cmd = new NpgsqlCommand(query, con);
        foreach (var (name, value) in parameters)
        {
            cmd.Parameters.AddWithValue(name, value);
        }

        try
        {
            var dataReader = await cmd.ExecuteReaderAsync();
            return ConvertResponse<T>(dataReader);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("SQL EXCEPTION: \n" + ex);
            return Result.WithMessages<List<T>>(new Error("Database error see logs"));
        }
    }

    public Task<Result<T>> GetSingle<T>(string query, Dictionary<string, object> parameters)
        => Get<T>(query, parameters)
            .ActAsync(output => Result.For(output.Single()));
}