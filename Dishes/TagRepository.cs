using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Dishes
{
    public class TagRepository
    {
        private static string _connectionString = "Data Tag=dishes.db";

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

        public void AddTag(Tag tag)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"insert into tags (Name) values ($name);";
            command.AddAllPropertiesAsParameters(tag);
            command.ExecuteNonQuery();
            command.CommandText = "select last_insert_rowid()";

            // The row ID is a 64-bit value - cast the Command result to an Int64.
            //
            var lastRowId64 = (long)command.ExecuteScalar();

            // Then grab the bottom 32-bits as the unique ID of the row.
            //
            var lastRowId = (int)lastRowId64;
            tag.Id = lastRowId;
        }

        public void DeleteTag(int tagId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"delete from tags where TagId=$id;";
            command.Parameters.AddWithValue("$id", tagId);
            command.ExecuteNonQuery();
        }
    }
}