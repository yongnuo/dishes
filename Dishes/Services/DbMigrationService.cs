using Dishes.Extensions;
using Microsoft.Data.Sqlite;

namespace Dishes.Services
{
    public class DbMigrationService
    {
        private static string _connectionString = "Data Source=dishes.db";

        public DbMigrationService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void VerifySchema()
        {
            var schemaVersion = GetSchemaVersion();
            switch (schemaVersion)
            {
                case 0:
                    SetSchemaTo1();
                    break;
                case 1:
                    return;
            }
        }

        private void SetSchemaTo1()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
CREATE TABLE [sources] (
  [SourceId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] text NOT NULL
);

insert into sources (Name) values (""Default Source"");

CREATE TABLE [tags] (
  [TagId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] text NOT NULL
);

CREATE TABLE [dishes] (
  [DishId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] text NOT NULL
, [Comment] text NULL
, [SourceId] int NOT NULL
, [Path] text NULL
, CONSTRAINT [FK_dishes_0_0] FOREIGN KEY ([SourceId]) REFERENCES [sources] ([SourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [dishtags] (
  [DishId] INTEGER NOT NULL
, [TagId] INTEGER NOT NULL
, CONSTRAINT [sqlite_autoindex_dishtags_1] PRIMARY KEY ([DishId],[TagId])
, CONSTRAINT [FK_dishtags_0_0] FOREIGN KEY ([DishId]) REFERENCES [dishes] ([DishId]) ON DELETE NO ACTION ON UPDATE NO ACTION
, CONSTRAINT [FK_dishtags_1_0] FOREIGN KEY ([TagId]) REFERENCES [tags] ([TagId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [dbinfo] (
  [DbInfoId] INTEGER PRIMARY KEY NOT NULL
, [Value] text NOT NULL
);

insert into dbinfo (DbInfoId, Value) values (1, 1);
";
            command.ExecuteNonQuery();
        }

        private int GetSchemaVersion()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"select * from dbinfo
where DbInfoId=1;";
            try
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return reader.MapToSchemaVersion();
                }
            }
            catch (SqliteException)
            {
                return 0;
            }

            return 0;
        }
    }
}