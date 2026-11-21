using System;
using System.Collections.Generic;
using System.Globalization;
using Gestor_de_Tareas.Modelos;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Tareas.Datos
{
    // Implements the four CRUD operations with parameterized ADO.NET commands.
    public class SqliteTaskRepository : ITaskRepository
    {
        public IReadOnlyList<TaskItem> GetAll()
        {
            var tasks = new List<TaskItem>();

            using var connection = Database.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Title, Description, Priority, Status, CreatedAt
                FROM Tasks
                ORDER BY Id DESC;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new TaskItem(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    (Priority)reader.GetInt32(3),
                    (Status)reader.GetInt32(4),
                    DateTime.Parse(reader.GetString(5), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)));
            }

            return tasks;
        }

        public int Create(TaskItem task)
        {
            using var connection = Database.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Tasks (Title, Description, Priority, Status, CreatedAt)
                VALUES ($title, $description, $priority, $status, $createdAt);
                SELECT last_insert_rowid();";
            AddTaskParameters(command, task);
            command.Parameters.AddWithValue("$createdAt", FormatDate(task.CreatedAt));

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void Update(TaskItem task)
        {
            using var connection = Database.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Tasks
                SET Title = $title, Description = $description, Priority = $priority, Status = $status
                WHERE Id = $id;";
            AddTaskParameters(command, task);
            command.Parameters.AddWithValue("$id", task.Id);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = Database.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Tasks WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);

            command.ExecuteNonQuery();
        }

        private static void AddTaskParameters(SqliteCommand command, TaskItem task)
        {
            command.Parameters.AddWithValue("$title", task.Title);
            command.Parameters.AddWithValue("$description", task.Description);
            command.Parameters.AddWithValue("$priority", (int)task.Priority);
            command.Parameters.AddWithValue("$status", (int)task.Status);
        }

        private static string FormatDate(DateTime date)
        {
            return date.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}
