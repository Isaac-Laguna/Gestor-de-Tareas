using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Tareas.Datos
{
    // Owns the connection string and creates the SQLite schema.
    public static class Database
    {
        private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "gestor_tareas.db");
        private static readonly string ConnectionString = "Data Source=" + FilePath;

        public static string Location => FilePath;

        public static SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public static void Initialize()
        {
            using var connection = CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title       TEXT    NOT NULL,
                    Description TEXT    NOT NULL,
                    Priority    INTEGER NOT NULL,
                    Status      INTEGER NOT NULL,
                    CreatedAt   TEXT    NOT NULL
                );";
            command.ExecuteNonQuery();
        }
    }
}
