using System.Text.Json;
using DSC.Toolkit.Core.Models;

namespace DSC.Toolkit.Core.Services;

public sealed class CampaignStorage
{
    public const int CurrentSchemaVersion = 1;
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
    public string DataRoot { get; }

    public CampaignStorage(string? dataRoot = null)
    {
        DataRoot = dataRoot ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSunChronicles", "DMGCompanion");
    }

    public string GetPath(Guid id) => Path.Combine(DataRoot, "Campaigns", $"{id:N}.dscampaign.json");

    public async Task SaveAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        campaign.SchemaVersion = CurrentSchemaVersion;
        campaign.ModifiedUtc = DateTimeOffset.UtcNow;
        var path = GetPath(campaign.Id);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        Directory.CreateDirectory(Path.Combine(DataRoot, "Backups"));
        var temporary = path + ".tmp";
        await using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            await JsonSerializer.SerializeAsync(stream, campaign, _json, cancellationToken);
        if (File.Exists(path))
        {
            var backup = Path.Combine(DataRoot, "Backups", $"{campaign.Id:N}-{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            File.Copy(path, backup, true);
            File.Move(temporary, path, true);
            RotateBackups(campaign.Id, 10);
        }
        else File.Move(temporary, path);
    }

    public async Task<Campaign> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(path);
        var campaign = await JsonSerializer.DeserializeAsync<Campaign>(stream, _json, cancellationToken)
            ?? throw new InvalidDataException("The campaign file contains no campaign data.");
        if (campaign.SchemaVersion > CurrentSchemaVersion) throw new NotSupportedException($"Schema {campaign.SchemaVersion} is newer than this application supports.");
        return campaign;
    }

    public async Task<Campaign> ImportAsync(string path, bool replaceIdentity = false, CancellationToken cancellationToken = default)
    {
        var campaign = await LoadAsync(path, cancellationToken);
        if (!replaceIdentity) campaign.Id = Guid.NewGuid();
        await SaveAsync(campaign, cancellationToken);
        return campaign;
    }

    public IEnumerable<string> Search(Campaign campaign, string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];
        return campaign.Records.Where(r =>
            r.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            r.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            r.Fields.Any(f => f.Key.Contains(query, StringComparison.OrdinalIgnoreCase) || f.Value.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .Select(r => $"{r.ModuleId} / {r.Name}");
    }

    private void RotateBackups(Guid id, int keep)
    {
        var folder = Path.Combine(DataRoot, "Backups");
        foreach (var old in Directory.EnumerateFiles(folder, $"{id:N}-*.json").OrderByDescending(File.GetCreationTimeUtc).Skip(keep)) File.Delete(old);
    }
}
