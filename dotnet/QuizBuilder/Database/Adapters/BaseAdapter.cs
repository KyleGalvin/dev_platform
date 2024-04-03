using Npgsql;
using QuizBuilder.Util;
using System.Data;

namespace QuizBuilder.Database.Adapters
{
    public class BaseAdapter
    {

        protected string _connectionString;
        public BaseAdapter()
        {
            _connectionString = $"Server={EnvironmentVars.GetPostgresHost()};Port={EnvironmentVars.GetPostgresPort()};User Id={EnvironmentVars.GetPostgresUser()};Password={EnvironmentVars.GetPostgresPassword()};Database={EnvironmentVars.GetDatabaseName()};SearchPath={EnvironmentVars.GetPostgresSchema()};";

        }
        public static IDbConnection OpenConnection(string connStr)
        {
            var conn = new NpgsqlConnection(connStr);
            conn.Open();
            return conn;
        }
    }
}
