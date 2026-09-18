using System.Text.Json.Serialization;
using DSC.Toolkit.Core.Services;

namespace DSC.Toolkit.Core.Models;

public sealed class Campaign
{
    public int SchemaVersion { get; set; } = 1;
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "New Campaign";
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<ToolRecord> Records { get; set; } = [];
    public List<Track> Tracks { get; set; } = [];
    public List<DiceRoll> RollHistory { get; set; } = [];
    public List<RecordLink> Links { get; set; } = [];
    [JsonExtensionData] public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }
}

public sealed class ToolRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ModuleId { get; set; } = "luminaris";
    public string ToolId { get; set; } = "campaign-foundation";
    public string Name { get; set; } = "Untitled Record";
    public List<string> Tags { get; set; } = [];
    public Dictionary<string, string> Fields { get; set; } = [];
    public DateTimeOffset ModifiedUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed record RecordLink(Guid SourceId, Guid TargetId, string Relationship);

public sealed class Track
{
    public const int MaxHistoryEntries = 1000;
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "New Track";
    public string Description { get; set; } = "";
    public string Scope { get; set; } = "Campaign";
    public int Minimum { get; set; }
    public int Maximum { get; set; } = 10;
    public int Current { get; set; }
    public List<TrackThreshold> Thresholds { get; set; } = [];
    public List<TrackChange> History { get; set; } = [];

    public void Change(int delta, string reason)
    {
        var previous = Current;
        Current = Math.Clamp(Current + delta, Minimum, Maximum);
        History.Add(new(DateTimeOffset.UtcNow, previous, Current, reason));
        if (History.Count > MaxHistoryEntries) History.RemoveAt(0);
    }

    public bool Undo()
    {
        if (History.Count == 0) return false;
        Current = History[^1].Previous;
        History.RemoveAt(History.Count - 1);
        return true;
    }
}

public sealed record TrackThreshold(int Value, string Effect);
public sealed record TrackChange(DateTimeOffset Timestamp, int Previous, int Current, string Reason);

public sealed record LockRecord(string World, string Guardian, string Sword, string LockState = "Intact", string Phenomena = "", string MortalSuccessor = "");

public static class CanonicalLocks
{
    public static IReadOnlyList<LockRecord> All { get; } =
    [
        new("Luminaris", "Luminar", "Shadowmourne"),
        new("Umbraria", "Umbrael", "Ashbringer"),
        new("Eternum", "Eternus", "Plaguebringer"),
        new("Solara", "Solara", "Scourgeborne"),
        new("Aetheria", "Aetherion", "Thundersoul"),
        new("Harmonia", "Harmonia", "Chaosbringer"),
        new("Celestia", "Celestian", "Felmourne")
    ];
}
