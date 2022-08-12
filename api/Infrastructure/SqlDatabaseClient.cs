using System;
using System.Collections.Generic;
using Npgsql;

namespace Dotnet2.Infrastructure.SqlDatabaseClient
{
    public static partial class SqlDatabaseClient
    {
        public static (List<T>, string) ProcessQuery<T>(string query)
        {
            using var con = new NpgsqlConnection("string");
            con.Open();
            using var cmd = new NpgsqlCommand(query, con);

            try
            {
                var dataReader = cmd.ExecuteReader();
                return (ConvertResult<T>(dataReader), "success");

            }
            catch (NpgsqlException ex)
            {
                return (new(), ex.ToString());
            }
        }

        public static (List<Object>, string) ProcessFreeQuery(string query)
        {
            using var con = new NpgsqlConnection("string");
            con.Open();
            using var cmd = new NpgsqlCommand(query, con);

            try
            {
                var dataReader = cmd.ExecuteReader();
                return (ConvertResult(dataReader), "success");
            }
            catch (NpgsqlException ex)
            {
                return (new(), ex.ToString());
            }
        }
    }
}