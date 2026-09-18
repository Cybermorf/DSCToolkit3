using System.Text.Json;
using DSC.Toolkit.Core.Models;

namespace DSC.Toolkit.Core.Services;

public sealed class CampaignStorage
{
    public const int CurrentSchemaVersion = 1;
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
    private readonly SemaphoreSlim _saveGate = new(1, 1);
    public string DataRoot { get; }

    public CampaignStorage(string? dataRoot = null)
    {
        DataRoot = dataRoot ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSunChronicles", "DMGCompanion");
    }

    public string GetPath(Guid id) => Path.Combine(DataRoot, "Campaigns", $"{id:N}.dscampaign.json");

    public async Task SaveAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        await _saveGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            campaign.SchemaVersion = CurrentSchemaVersion;
            campaign.ModifiedUtc = DateTimeOffset.UtcNow;
            var path = GetPath(campaign.Id);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            Directory.CreateDirectory(Path.Combine(DataRoot, "Backups"));
            var temporary = path + ".tmp";

            // Serialize away from the UI thread. The save gate prevents concurrent saves
            // from overwriting each other's temporary file.
            var payload = await Task.Run(
                () => JsonSerializer.SerializeToUtf8Bytes(campaign, _json),
                cancellationToken).ConfigureAwait(false);

            await using (var stream = new FileStream(
                temporary, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024,
                FileOptions.Asynchronous | FileOptions.SequentialScan))
            {
                await stream.WriteAsync(payload, cancellationToken).ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            // File.Copy, File.Move, directory enumeration, and deletion are synchronous
            // APIs; keep them off the WinUI thread.
            await Task.Run(() => CommitFiles(path, temporary, campaign.Id), cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _saveGate.Release();
        }
    }

    public async Task<Campaign> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        var campaign = await JsonSerializer.DeserializeAsync<Campaign>(stream, _json, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidDataException("The campaign file contains no campaign data.");
        if (campaign.SchemaVersion > CurrentSchemaVersion) throw new NotSupportedException($"Schema {campaign.SchemaVersion} is newer than this application supports.");
        return campaign;
    }

    public async Task<Campaign> ImportAsync(string path, bool replaceIdentity = false, CancellationToken cancellationToken = default)
    {
        var campaign = await LoadAsync(path, cancellationToken).ConfigureAwait(false);
        if (!replaceIdentity) campaign.Id = Guid.NewGuid();
        await SaveAsync(campaign, cancellationToken).ConfigureAwait(false);
        return campaign;
    }

    public IEnumerable<string> Search(Campaign campaign, string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];
        return campaign.Records.Where(record => Matches(record, query))
            .Select(record => $"{record.ModuleId} / {record.Name}");
    }

    public int CountMatches(Campaign campaign, string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return 0;
        return campaign.Records.Count(record => Matches(record, query));
    }

    private static bool Matches(ToolRecord record, string query) =>
        record.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        record.Tags.Any(tag => tag.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
        record.Fields.Any(field => field.Key.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                   field.Value.Contains(query, StringComparison.OrdinalIgnoreCase));

    private void CommitFiles(string path, string temporary, Guid id)
    {
        if (File.Exists(path))
        {
            var backup = Path.Combine(DataRoot, "Backups", $"{id:N}-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json");
            File.Copy(path, backup, true);
            File.Move(temporary, path, true);
            RotateBackups(id, 10);
        }
        else
        {
            File.Move(temporary, path);
        }
    }

    private void RotateBackups(Guid id, int keep)
    {
        var folder = Path.Combine(DataRoot, "Backups");
        foreach (var old in Directory.EnumerateFiles(folder, $"{id:N}-*.json")
                     .OrderByDescending(File.GetLastWriteTimeUtc)
                     .Skip(keep))
        {
            File.Delete(old);
        }
    }
}
