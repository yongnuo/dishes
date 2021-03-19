using System.Collections.Generic;
using Dishes.Extensions;
using Dishes.Models;
using Microsoft.Data.Sqlite;

namespace Dishes.Repositories
{
    public class TagRepository
    {
        private readonly string _connectionString;

        public TagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Tag> LoadTags()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"select * from tags";
            var tags = new List<Tag>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(reader.MapToTag());
            }

            return tags;
        }

        public void AddTag(Tag tag)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"insert into tags (Name) values ($name);";
            command.AddAllPropertiesAsParameters(tag);
            command.ExecuteNonQuery();
            tag.Id = connection.GetLastRowId();
        }

        public void UpdateTag(Tag tag)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"update tags set
Name=$name
where TagId=$id;";
            command.AddAllPropertiesAsParameters(tag);
            command.ExecuteNonQuery();
        }

        public void DeleteTag(int tagId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"delete from dishtags where TagId=$id;
delete from tags where TagId=$id;";
            command.Parameters.AddWithValue("$id", tagId);
            command.ExecuteNonQuery();
        }
    }
}