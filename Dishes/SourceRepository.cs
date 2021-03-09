using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Dishes
{
    public class SourceRepository
    {
        private static string _connectionString = "Data Source=dishes.db";

        public SourceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Source> LoadSources()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"select * from sources";
            var sources = new List<Source>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                sources.Add(reader.MapToSource());
            }

            return sources;
        }

        public void UpdateSource(Source source)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"update sources set
Name=$name
where SourceId=$id;";
            command.AddAllPropertiesAsParameters(source);
            command.ExecuteNonQuery();
        }

        public void AddSource(Source source)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"insert into sources (Name) values ($name);";
            command.AddAllPropertiesAsParameters(source);
            command.ExecuteNonQuery();
            command.CommandText = "select last_insert_rowid()";

            // The row ID is a 64-bit value - cast the Command result to an Int64.
            //
            var lastRowId64 = (long)command.ExecuteScalar();

            // Then grab the bottom 32-bits as the unique ID of the row.
            //
            var lastRowId = (int)lastRowId64;
            source.Id = lastRowId;
        }

        public void DeleteSource(int sourceId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"delete from sources where SourceId=$id;";
            command.Parameters.AddWithValue("$id", sourceId);
            command.ExecuteNonQuery();
        }
    }
}