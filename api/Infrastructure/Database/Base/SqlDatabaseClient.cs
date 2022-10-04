using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Npgsql;
using SpoRE.Infrastructure.Database.Stage;
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

    public Task<Result<List<T>>> Get<T>(string query, IEnumerable<QueryParameter> parameters)
        => Get<T>(new List<Query>() { new(query, parameters) });

    internal async Task<Result<List<T>>> Get<T>(List<Query> batch2)
    {
        using var con = new NpgsqlConnection(_configuration.MyDbConnection);
        con.Open();
        using var batch = new NpgsqlBatch(con);
        foreach (var query in batch2)
        {
            var batchCommand = new NpgsqlBatchCommand(query.QueryString);
            foreach (var p in query.Parameters)
            {
                batchCommand.Parameters.AddWithValue(p.Name, p.Value);
            }
            batch.BatchCommands.Add(batchCommand);
        }
        try
        {
            var dataReader = await batch.ExecuteReaderAsync();
            // https://stackoverflow.com/questions/54691579/preparing-statements-and-batching-in-npgsql veel simpelere data processing
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