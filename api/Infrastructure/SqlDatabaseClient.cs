using Npgsql;

namespace SpoRE.Infrastructure.SqlDatabaseClient
{
    public static partial class SqlDatabaseClient
    {
        public static (List<T>, string) ProcessQuery<T>(string query) // zoek de tutorial op die ik volgde als je dit wilt snappen
        { //<T> betekent dat het returntype een argument is
            var SqlConnectionString = Secrets.SqlConnectionString;
            using var con = new NpgsqlConnection(SqlConnectionString);
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

        public static (List<Object>, string) ProcessFreeQuery(string query) // deze bestaat alleen voor de admin page met open queries
        {// kan ik wss fuseren met bovenstaande functie
            var SqlConnectionString = Secrets.SqlConnectionString;
            using var con = new NpgsqlConnection(SqlConnectionString);
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