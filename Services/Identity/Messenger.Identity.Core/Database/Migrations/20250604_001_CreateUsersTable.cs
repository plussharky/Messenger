using FluentMigrator;

namespace Messenger.Identity.Core.Database.Migrations;

[Migration(20250604_001)]
internal sealed class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create.Table("users").InSchema("public")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}
