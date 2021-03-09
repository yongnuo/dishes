using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Dishes
{
    public class DishRepository
    {
        private static string _connectionString = "Data Source=dishes.db";

        public DishRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Dish> LoadDishes()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"select * from dishes";
            var dishes = new List<Dish>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                dishes.Add(reader.MapToDish());
            }

            return dishes;
        }

        public void AddDish(Dish dish)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"insert into dishes (Name, Comment, SourceId, Path) values ($name, $comment, $sourceId, $path);";
            command.AddAllPropertiesAsParameters(dish);
            command.ExecuteNonQuery();
            command.CommandText = "select last_insert_rowid()";

            // The row ID is a 64-bit value - cast the Command result to an Int64.
            //
            var lastRowId64 = (long)command.ExecuteScalar();

            // Then grab the bottom 32-bits as the unique ID of the row.
            //
            var lastRowId = (int)lastRowId64;
            dish.Id = lastRowId;
        }

        public void UpdateDish(Dish dish)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"update dishes set
Name=$name,
Comment=$comment,
SourceId=$sourceId, 
Path=$path
where DishId=$id;";
            command.AddAllPropertiesAsParameters(dish);
            command.ExecuteNonQuery();
        }

        public void DeleteDish(int dishId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"delete from dishes where DishId=$id;";
            command.Parameters.AddWithValue("$id", dishId);
            command.ExecuteNonQuery();
        }
    }
}