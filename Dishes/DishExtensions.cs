using Microsoft.Data.Sqlite;

namespace Dishes
{
    public static class DishExtensions
    {
        public static Dish MapToDish(this SqliteDataReader reader)
        {
            var dish = new Dish();
            dish.Id = reader.GetInt32(0);
            dish.Name = reader.GetString(1);
            if(!reader.IsDBNull(2))
                dish.Comment = reader.GetString(2);
            dish.SourceId = reader.GetInt32(3);
            if (!reader.IsDBNull(4))
                dish.Path = reader.GetString(4);
            return dish;
        }

        public static void AddAllPropertiesAsParameters(this SqliteCommand command, Dish dish)
        {
            command.Parameters.AddWithValue("$name", dish.Name);
            command.Parameters.AddWithValue("$comment", dish.Comment);
            command.Parameters.AddWithValue("$sourceId", dish.Source.Id);
            command.Parameters.AddWithValue("$path", dish.Path);
            command.Parameters.AddWithValue("$id", dish.Id);
        }

        public static void AddAllPropertiesAsParameters(this SqliteCommand command, Source source)
        {
            command.Parameters.AddWithValue("$name", source.Name);
            command.Parameters.AddWithValue("$id", source.Id);
        }
    }
}