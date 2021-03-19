using System.Collections.Generic;
using Dishes.Interfaces;

namespace Dishes.Models
{
    public class Dish : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public int SourceId { get; set; }
        public string Path { get; set; }
        public Source Source { get; set; }
        public List<Tag> Tags { get; set; }

        public Dish()
        {
            Comment = string.Empty;
            Path = string.Empty;
            Name = string.Empty;
        }
    }
}