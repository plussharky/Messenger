using FluentMigrator.Runner.VersionTableInfo;

namespace Messenger.Identity.Core.Repository.Migrations;

internal sealed class CustomVersionTableMetaData : IVersionTableMetaData
{
    public string SchemaName => "public";

    public string TableName => "VersionInfo";

    public string ColumnName => "Version";

    public string DescriptionColumnName => "Description";

    public string AppliedOnColumnName => "AppliedOn";

    public string? UniqueIndexName => null;

    public bool CreateWithPrimaryKey => true;

    public bool OwnsSchema => false;
}
