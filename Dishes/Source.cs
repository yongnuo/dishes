namespace Dishes
{
    public class Source : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Tag : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}