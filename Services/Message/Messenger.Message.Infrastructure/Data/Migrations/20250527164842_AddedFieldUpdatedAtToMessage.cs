using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messenger.Messages.Infrastructure.Data.Migrations;

/// <inheritdoc />
public sealed partial class AddedFieldUpdatedAtToMessage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "Messages",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "Messages");
    }
}
