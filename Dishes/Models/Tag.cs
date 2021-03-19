using Dishes.Interfaces;

namespace Dishes.Models
{
    public class Tag : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}