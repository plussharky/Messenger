using FluentMigrator;

namespace Messenger.Identity.Core.Database.Migrations;

[Migration(20250604_003, TransactionBehavior.Default)]
public sealed class CreateRefreshTokensTable : Migration
{
    public override void Up()
    {
        Create.Table("refresh_tokens").InSchema("public")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("user_id").AsGuid().NotNullable().ForeignKey("fk_refresh_tokens_users", "public", "users", "id")
            .WithColumn("token").AsString(512).NotNullable().Unique()
            .WithColumn("expires_at").AsDateTimeOffset().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable()
            .WithColumn("is_revoked").AsBoolean().NotNullable()
            .WithColumn("revoked_at").AsDateTimeOffset().Nullable()
            .WithColumn("replaced_by_token").AsString(512).Nullable();
    }

    public override void Down()
    {
        Delete.Table("refresh_tokens");
    }
}
