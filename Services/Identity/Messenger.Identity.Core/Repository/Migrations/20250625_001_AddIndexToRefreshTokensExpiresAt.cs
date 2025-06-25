using FluentMigrator;

namespace Messenger.Identity.Core.Repository.Migrations;

[Migration(20250625_001, TransactionBehavior.Default)]
public sealed class AddIndexToRefreshTokensExpiresAt : Migration
{
    public override void Up()
    {
        Create.Index("ix_refresh_tokens_expires_at")
            .OnTable("refresh_tokens")
            .InSchema("public")
            .OnColumn("expires_at")
            .Ascending();
    }

    public override void Down()
    {
        Delete.Index("ix_refresh_tokens_expires_at")
            .OnTable("refresh_tokens")
            .InSchema("public");
    }
}