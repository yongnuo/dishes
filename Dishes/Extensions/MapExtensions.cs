using System;
using Dishes.Models;
using Microsoft.Data.Sqlite;

namespace Dishes.Extensions
{
    public static class MapExtensions
    {
        public static Source MapToSource(this SqliteDataReader reader)
        {
            var source = new Source();
            source.Id = reader.GetInt32(0);
            source.Name = reader.GetString(1);
            return source;
        }

        public static Tag MapToTag(this SqliteDataReader reader)
        {
            var tag = new Tag();
            tag.Id = reader.GetInt32(0);
            tag.Name = reader.GetString(1);
            return tag;
        }

        public static Dish MapToDish(this SqliteDataReader reader)
        {
            var dish = new Dish();
            dish.Id = reader.GetInt32(0);
            dish.Name = reader.GetString(1);
            if (!reader.IsDBNull(2))
                dish.Comment = reader.GetString(2);
            dish.SourceId = reader.GetInt32(3);
            if (!reader.IsDBNull(4))
                dish.Path = reader.GetString(4);
            return dish;
        }

        public static DishTag MapToDishTag(this SqliteDataReader reader)
        {
            var dishTag = new DishTag();
            dishTag.DishId = reader.GetInt32(0);
            dishTag.TagId = reader.GetInt32(1);
            return dishTag;
        }

        public static int MapToSchemaVersion(this SqliteDataReader reader)
        {
            var schemaVersion = reader.GetString(1);
            return Convert.ToInt32(schemaVersion);
        }
    }
}