using System.Data;
using MySql.Data.MySqlClient;

namespace SHBank
{
    public class ConnectionHelper
    {
        private static MySqlConnection _connection;
        private static readonly string Server = "127.0.0.1";
        private static readonly string Username = "root";
        private static readonly string Password = "";
        private static readonly string Database = "sh_bank";
        private static string _connectionString = "server={0};uid={1};pwd={2};database={3};SslMode=none";

        public static MySqlConnection GetInstance()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = 
                    new MySqlConnection(string.Format(_connectionString, Server, Username, Password, Database));
            }

            return _connection;
        }
    }
}