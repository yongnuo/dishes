using System;
using Dishes.Models;
using Microsoft.Data.Sqlite;

namespace Dishes.Extensions
{
    public static class DbExtensions
    {
        public static int GetLastRowId(this SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "select last_insert_rowid()";

            // The row ID is a 64-bit value - cast the Command result to an Int64.
            //
            var lastRowId64 = (long)command.ExecuteScalar();

            // Then grab the bottom 32-bits as the unique ID of the row.
            //
            var lastRowId = (int)lastRowId64;
            return lastRowId;
        }

        public static void AddAllPropertiesAsParameters(this SqliteCommand command, Dish dish)
        {
            command.Parameters.AddWithValue("$name", dish.Name);
            command.Parameters.AddWithValue("$comment", dish.Comment != null ? (object) dish.Comment : DBNull.Value);
            command.Parameters.AddWithValue("$sourceId", dish.Source.Id);
            if(dish.Path == null)
                command.Parameters.AddWithValue("$path", DBNull.Value);
            else
                command.Parameters.AddWithValue("$path", dish.Path);
            command.Parameters.AddWithValue("$id", dish.Id);
        }

        public static void AddAllPropertiesAsParameters(this SqliteCommand command, Source source)
        {
            command.Parameters.AddWithValue("$name", source.Name);
            command.Parameters.AddWithValue("$id", source.Id);
        }

        public static void AddAllPropertiesAsParameters(this SqliteCommand command, Tag tag)
        {
            command.Parameters.AddWithValue("$name", tag.Name);
            command.Parameters.AddWithValue("$id", tag.Id);
        }
    }
}