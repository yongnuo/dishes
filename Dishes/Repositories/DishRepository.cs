using System.Collections.Generic;
using Dishes.Extensions;
using Dishes.Models;
using Microsoft.Data.Sqlite;

namespace Dishes.Repositories
{
    public class DishRepository
    {
        private readonly string _connectionString;

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


        public List<DishTag> LoadDishTags()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"select * from dishtags";
            var dishTags = new List<DishTag>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                dishTags.Add(reader.MapToDishTag());
            }
            return dishTags;
        }

        public void AddDish(Dish dish)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText =
                @"insert into dishes (Name, Comment, SourceId, Path) values ($name, $comment, $sourceId, $path);";
            command.AddAllPropertiesAsParameters(dish);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            dish.Id = connection.GetLastRowId();
            UpdateSaveDishTags(command, dish);
            transaction.Commit();
        }

        public void UpdateDish(Dish dish)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();

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
            command.Parameters.Clear();
            UpdateSaveDishTags(command, dish);
            transaction.Commit();
        }

        public void DeleteDish(int dishId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"delete from dishtags where DishId=$id;
delete from dishes where DishId=$id;";
            command.Parameters.AddWithValue("$id", dishId);
            command.ExecuteNonQuery();
        }

        private void UpdateSaveDishTags(SqliteCommand command, Dish dish)
        {
            command.CommandText = @"delete from dishtags 
where DishId=$id";
            command.Parameters.AddWithValue("$id", dish.Id);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            foreach (var dishTag in dish.Tags)
            {
                command.CommandText = @"insert into dishtags
(DishId, TagId)
values
($dishId, $tagId)";
                command.Parameters.AddWithValue("$dishId", dish.Id);
                command.Parameters.AddWithValue("$tagId", dishTag.Id);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
        }
    }
}