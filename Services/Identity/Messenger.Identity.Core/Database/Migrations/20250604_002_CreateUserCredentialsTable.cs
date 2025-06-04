using FluentMigrator;

namespace Messenger.Identity.Core.Database.Migrations;

[Migration(20250604_002, TransactionBehavior.Default)]
public sealed class CreateUserCredentialsTable : Migration
{
    public override void Up()
    {
        Create.Table("user_credentials").InSchema("public")
            .WithColumn("user_id").AsGuid().PrimaryKey().ForeignKey("fk_user_credentials_users", "public", "users", "id")
            .WithColumn("email").AsString(255).NotNullable().Unique()
            .WithColumn("password_hash").AsString(255).NotNullable()
            .WithColumn("salt").AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("user_credentials");
    }
}
