using Dishes.Interfaces;

namespace Dishes.Models
{
    public class Source : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}