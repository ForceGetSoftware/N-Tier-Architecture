namespace N_Tier.Application.Models;

public class ReplaceManyResult
{
    public long MatchedCount { get; }
    public long ModifiedCount { get; }
    public int UpsertedCount { get; }

    public ReplaceManyResult(long matchedCount, long modifiedCount, int upsertedCount)
    {
        MatchedCount = matchedCount;
        ModifiedCount = modifiedCount;
        UpsertedCount = upsertedCount;
    }
}
