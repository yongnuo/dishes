using Microsoft.Data.Sqlite;

namespace Dishes
{
    public static class SourceExtensions
    {
        public static Source MapToSource(this SqliteDataReader reader)
        {
            var source = new Source();
            source.Id = reader.GetInt32(0);
            source.Name = reader.GetString(1);
            return source;
        }
    }
}