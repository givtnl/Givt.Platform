using AutoMapper;
using Givt.Platform.CockroachDB;
using Givt.Platform.EF.Enums;
using Givt.Platform.EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Givt.Platform.EF.DbContexts;

public class CommonContext : DbContext
{
    private readonly IMapper _mapper;
    public CommonContext(DbContextOptions options, IMapper mapper)
        : base(options)
    {
        _mapper = mapper;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // support the generation of ON UPDATE SQL on columns
        optionsBuilder.ReplaceService<IMigrationsSqlGenerator, CockroachMigrationsSqlGenerator>();
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    #region Auditing and logging

    /* Automatically update the IAuditBasic properties and process ILoggedEntity without any developer action.
     * This is done in 3 phases:
     * 1: SetAuditData: update Created/Modified time stamps
     * 2: GetLoggedEntities:
     *  a) Scan the changed entities, update dates, and create log entries(IHistory). Safeguard the properties of those entities 
     *     on the verge of deletion(after base.SaveXxxx() they will be lost).
     *  b) base.SaveXxx(). After the write, this will fetch the new ID, (new) ConcurrencyToken etc.from the database.
     * 3: WriteLoggedEntities
     * a) Scan the changed entities again, and get the properties of Created or Updated entities(including changed IDs etc.)
     *  b) base.SaveXxx(). This now only stores the history log.
     */

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var now = DateTime.UtcNow;
        // set last modified info etc.
        SetAuditData(now);
        // check for changes in ILoggedEntity entries.
        var historyList = GetLoggedEntities(now);
        // store what we can
        var count = base.SaveChanges(acceptAllChangesOnSuccess);
        // is there more to write?
        if (historyList?.Count > 0)
        {
            // Store history linked with new ID's etc
            WriteLoggedEntities(historyList);
            // save the history data
            count += base.SaveChanges(acceptAllChangesOnSuccess);
        }
        return count;
    }

    // public override int SaveChanges() // calls SaveChanges (above) from base class

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        // set last modified info etc.
        SetAuditData(now);
        // check for changes in IAuditBasic entries.
        var historyList = GetLoggedEntities(now);
        // store what we can
        var count = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        // is there more to write?
        if (historyList?.Count > 0)
        {
            // Store history linked with new ID's etc
            WriteLoggedEntities(historyList);
            // save the history data            
            count += await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        return count;
    }
    // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) // calls SaveChanges (above) from base class

    private void SetAuditData(DateTime now)
    {
        ChangeTracker.DetectChanges();
        // Add Created/Modified data for simple Audit entries
        var auditEntries = ChangeTracker
            .Entries<IAuditBasic>()
            .Where(e =>
                e.State == EntityState.Added ||
                e.State == EntityState.Modified);

        foreach (var entry in auditEntries)
        {
            var entity = entry.Entity;
            if (entry.State == EntityState.Added)
                entity.Created = now;
            entity.Modified = now; // added or modified
        }
    }

    private List<KeyValuePair<IHistory, ILoggedEntity>> GetLoggedEntities(DateTime now)
    {
        // keep a list history records
        var historyList = new List<KeyValuePair<IHistory, ILoggedEntity>>();
        var auditableEntries = ChangeTracker
                .Entries<ILoggedEntity>()
                .Where(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted)
                .ToList();
        // create history entries
        foreach (var auditEntry in auditableEntries)
        {
            EntityLogReason reason = auditEntry.State switch
            {
                EntityState.Added => EntityLogReason.Created,
                EntityState.Deleted => EntityLogReason.Deleted,
                _ => EntityLogReason.Updated,
            };

            // for Deleted entities: copy data before save -- before the actual record is deleted. Otherwise create an empty shell to collect data after the write
            IHistory historyEntry = (auditEntry.State == EntityState.Deleted) ?
                (IHistory)_mapper.Map(auditEntry.Entity, auditEntry.Entity.GetType(), auditEntry.Entity.HistoryEntityType) :
                (IHistory)Activator.CreateInstance(auditEntry.Entity.HistoryEntityType);
            if (historyEntry is not null)
            {
                historyEntry.Modified = now;
                historyEntry.Reason = reason;
                historyList.Add(new KeyValuePair<IHistory, ILoggedEntity>(historyEntry, auditEntry.Entity));
            }
        }
        return historyList;
    }

    private void WriteLoggedEntities(List<KeyValuePair<IHistory, ILoggedEntity>> historyList)
    {
        // copy data for new and updated entities (at this point the foreign key IDs are updated)
        foreach (var kvp in historyList)
        {
            var historyEntry = kvp.Key;
            var entity = kvp.Value;
            // for Created and Updated entities: copy data after save, after database has created IDs etc.
            if (historyEntry.Reason != EntityLogReason.Deleted)
                _mapper.Map(entity, historyEntry, entity.GetType(), entity.HistoryEntityType);
            // add to context for saving
            Add(historyEntry);
        }
    }

    #endregion
}
