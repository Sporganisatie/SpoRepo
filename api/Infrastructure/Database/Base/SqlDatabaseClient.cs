using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Npgsql;
using SpoRE.Models;
using SpoRE.Models.Settings;

namespace SpoRE.Infrastructure.Base;

public record QueryParameter(object Value, [CallerArgumentExpression("Value")] string Name = ""); // TODO beperken tot string en nummers ipv object?

public partial class SqlDatabaseClient
{
    private AppSettings _configuration;
    public SqlDatabaseClient(IOptions<AppSettings> configuration)
    {
        _configuration = configuration.Value;
    }

    public async Task<Result<List<T>>> Get<T>(string query, IEnumerable<QueryParameter> parameters)
    {
        using var con = new NpgsqlConnection(_configuration.MyDbConnection);
        con.Open();
        using var cmd = new NpgsqlCommand(query, con);

        foreach (var p in parameters)
        {
            cmd.Parameters.AddWithValue(p.Name, p.Value);
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

    public Task<Result<T>> GetSingle<T>(string query, object value, [CallerArgumentExpression("value")] string name = "")
        => GetSingle<T>(query, new List<QueryParameter> { new(value, name) });


    public Task<Result<T>> GetSingle<T>(string query, IEnumerable<QueryParameter> parameters)
        => Get<T>(query, parameters)
            .ActAsync(output => Result.For(output.Single()));
}