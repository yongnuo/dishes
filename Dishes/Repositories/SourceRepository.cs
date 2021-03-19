using System.Collections.Generic;
using Dishes.Extensions;
using Dishes.Models;
using Microsoft.Data.Sqlite;

namespace Dishes.Repositories
{
    public class SourceRepository
    {
        private readonly string _connectionString;

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

        public void AddSource(Source source)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"insert into sources (Name) values ($name);";
            command.AddAllPropertiesAsParameters(source);
            command.ExecuteNonQuery();
            source.Id = connection.GetLastRowId();
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