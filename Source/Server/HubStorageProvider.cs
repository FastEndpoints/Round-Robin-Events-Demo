using FastEndpoints;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace PublisherServer;

public class HubStorageProvider : IEventHubStorageProvider<PubEventRecord>
{
    private readonly DbContext db;

    public HubStorageProvider(DbContext db)
    {
        this.db = db;
    }

    public async ValueTask<IEnumerable<string>> RestoreSubscriberIDsForEventTypeAsync(SubscriberIDRestorationParams<PubEventRecord> p)
    {
        return await db
            .Queryable<PubEventRecord>()
            .Where(p.Match)
            .Select(p.Projection)
            .Distinct()
            .ToListAsync();
    }

    public async ValueTask StoreEventAsync(PubEventRecord r, CancellationToken ct)
    {
        await db.SaveAsync(r, ct);
    }

    public async ValueTask<IEnumerable<PubEventRecord>> GetNextBatchAsync(PendingRecordSearchParams<PubEventRecord> p)
    {
        return await db
            .Find<PubEventRecord>()
            .Match(p.Match)
            .Sort(e => e.ID, Order.Ascending)
            .Limit(p.Limit)
            .ExecuteAsync(p.CancellationToken);
    }

    public async ValueTask MarkEventAsCompleteAsync(PubEventRecord r, CancellationToken ct)
    {
        await db
            .Update<PubEventRecord>()
            .MatchID(r.ID)
            .Modify(e => e.IsComplete, true)
            .ExecuteAsync(ct);
    }

    public async ValueTask PurgeStaleRecordsAsync(StaleRecordSearchParams<PubEventRecord> p)
    {
        await db.DeleteAsync(p.Match, p.CancellationToken);
    }
}