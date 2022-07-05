using Givt.Platform.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Givt.Platform.EF.Configurations;

public class OptimisticLockConfiguration<TEntityAudit> : EntityBaseConfiguration<TEntityAudit>
    where TEntityAudit : EntityLockAudit<DateTime>
{
    public override void Configure(EntityTypeBuilder<TEntityAudit> builder)
    {
        base.Configure(builder);
        // override property default settings
        // MariaDB
        //builder
        //    .Property(e => e.ConcurrencyToken)
        //    .IsRowVersion();
        // CockroachDB: ConcurrencyToken TIMESTAMPTZ DEFAULT now() ON UPDATE now().
        // Note: ON UPDATE on a Column is not standard SQL. Special support for migrations is needed.
        builder
            .Property(e => e.ConcurrencyToken)
            .HasDefaultValueSql("now() ON UPDATE now()")
            .IsConcurrencyToken();
    }
}


