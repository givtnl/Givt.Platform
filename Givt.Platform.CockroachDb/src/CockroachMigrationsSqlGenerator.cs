using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace Givt.Platform.CockroachDB;

public class CockroachMigrationsSqlGenerator : NpgsqlMigrationsSqlGenerator
{
    public CockroachMigrationsSqlGenerator(
        MigrationsSqlGeneratorDependencies dependencies,
        INpgsqlSingletonOptions npgsqlSingletonOptions)
        : base(dependencies, npgsqlSingletonOptions)
    {
    }

    protected override void DefaultValue(object defaultValue, string defaultValueSql, string columnType, MigrationCommandListBuilder builder)
    {
        const string ON_UPDATE_MARKER = "ON UPDATE";
        //Check.NotNull(builder, nameof(builder));

        if (defaultValueSql != null)
        {
            var p = defaultValueSql.IndexOf(ON_UPDATE_MARKER, StringComparison.InvariantCultureIgnoreCase);
            if (p >= 0)
            {
                if (p > 0) // not 0, so there is an actual DEFAULT string
                {
                    builder
                        .Append(" DEFAULT (")
                        .Append(defaultValueSql[..p].Trim())
                        .Append(")");
                }
                builder
                    .Append(" ON UPDATE (")
                    .Append(defaultValueSql[(p + ON_UPDATE_MARKER.Length)..].Trim())
                    .Append(")");
                return;
            }
        }
        base.DefaultValue(defaultValue, defaultValueSql, columnType, builder);
    }
}