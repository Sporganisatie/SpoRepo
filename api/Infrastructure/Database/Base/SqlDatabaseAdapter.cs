using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Npgsql;
using SpoRE.Infrastructure.Database.Stage;
using SpoRE.Models;
using SpoRE.Models.Settings;

namespace SpoRE.Infrastructure.Base;

public record QueryParameter(object Value, [CallerArgumentExpression("Value")] string Name = ""); // TODO beperken tot string en nummers ipv object?

public partial class SqlDatabaseAdapter
{
    private AppSettings _configuration;
    public SqlDatabaseAdapter(IOptions<AppSettings> configuration)
    {
        _configuration = configuration.Value;
    }

    public Task<Result<List<T>>> Get<T>(string query, IEnumerable<QueryParameter> parameters)
        => Get<T>(new List<Query>() { new(query, parameters) });

    internal async Task<Result<List<T>>> Get<T>(List<Query> queryBatch)
    {
        using var con = new NpgsqlConnection(_configuration.MyDbConnection);
        con.Open();
        using var slqBatch = new NpgsqlBatch(con);
        foreach (var query in queryBatch)
        {
            var batchCommand = new NpgsqlBatchCommand(query.QueryString);
            foreach (var p in query.Parameters)
            {
                batchCommand.Parameters.AddWithValue(p.Name, p.Value);
            }
            slqBatch.BatchCommands.Add(batchCommand);
        }
        try
        {
            var dataReader = await slqBatch.ExecuteReaderAsync();
            return ConvertResponse<T>(dataReader); // TODO moet wss anders afhankelijk van aantal queries, met ingebouwde functies
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