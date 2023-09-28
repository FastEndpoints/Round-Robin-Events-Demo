using FastEndpoints;
using MongoDB.Entities;

namespace SubscriberClient;

public class SubscriberStorageProvider : IEventSubscriberStorageProvider<SubEventRecord>
{
    private readonly DbContext db;

    public SubscriberStorageProvider(DbContext db)
    {
        this.db = db;
    }

    public async ValueTask StoreEventAsync(SubEventRecord r, CancellationToken ct)
    {
        r.ExpireOn = DateTime.UtcNow.AddHours(24); //override default expiry time
        await db.SaveAsync(r, ct);
    }

    public async ValueTask<IEnumerable<SubEventRecord>> GetNextBatchAsync(PendingRecordSearchParams<SubEventRecord> p)
    {
        return await db
            .Find<SubEventRecord>()
            .Match(p.Match)
            .Sort(e => e.ID, Order.Ascending)
            .Limit(p.Limit)
            .ExecuteAsync(p.CancellationToken);
    }

    public async ValueTask MarkEventAsCompleteAsync(SubEventRecord r, CancellationToken ct)
    {
        //throw new InvalidOperationException("testing exception receiver!");

        await db
            .Update<SubEventRecord>()
            .MatchID(r.ID)
            .Modify(e => e.IsComplete, true)
            .ExecuteAsync(ct);
    }

    public async ValueTask PurgeStaleRecordsAsync(StaleRecordSearchParams<SubEventRecord> p)
    {
        await db.DeleteAsync(p.Match);
    }
}