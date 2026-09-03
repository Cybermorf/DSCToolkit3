using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DSC.Toolkit.Core.Models;
using DSC.Toolkit.Core.Services;

namespace DSC.Toolkit.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly CampaignStorage _storage = new();
    private readonly IDiceService _dice = new DiceService();

    public IReadOnlyList<ModuleDefinition> Modules => ModuleCatalog.All;
    public ObservableCollection<ToolRecord> VisibleRecords { get; } = [];
    public ObservableCollection<DiceRoll> Rolls { get; } = [];
    public IReadOnlyList<LockRecord> Locks => CanonicalLocks.All;

    [ObservableProperty] private Campaign _campaign = new();
    [ObservableProperty] private ModuleDefinition _selectedModule = ModuleCatalog.All[0];
    [ObservableProperty] private ToolDefinition? _selectedTool;
    [ObservableProperty] private ToolRecord? _selectedRecord;
    [ObservableProperty] private string _diceExpression = "1d20+0";
    [ObservableProperty] private string _diceLabel = "";
    [ObservableProperty] private string _status = "Ready";
    [ObservableProperty] private string _searchText = "";

    public MainViewModel() => SelectedTool = SelectedModule.Tools[0];

    partial void OnSelectedModuleChanged(ModuleDefinition value)
    {
        SelectedTool = value.Tools[0];
        RefreshRecords();
    }

    [RelayCommand]
    private void NewCampaign()
    {
        Campaign = new Campaign();
        VisibleRecords.Clear();
        Rolls.Clear();
        Status = "New campaign created";
    }

    [RelayCommand]
    private void AddRecord()
    {
        if (SelectedTool is null) return;
        var record = new ToolRecord
        {
            ModuleId = SelectedModule.Id,
            ToolId = SelectedTool.Id,
            Name = $"New {SelectedTool.Name}",
            Fields = SelectedTool.Fields.ToDictionary(field => field, _ => "")
        };
        Campaign.Records.Add(record);
        VisibleRecords.Add(record);
        SelectedRecord = record;
        Status = "Record added - save when ready";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await _storage.SaveAsync(Campaign);
        Status = $"Saved {DateTime.Now:t}";
    }

    [RelayCommand]
    private void Roll(string? modeName)
    {
        try
        {
            var mode = Enum.TryParse<RollMode>(modeName, true, out var parsed) ? parsed : RollMode.Normal;
            var result = _dice.Roll(DiceExpression, DiceLabel, mode);
            Campaign.RollHistory.Add(result);
            Rolls.Insert(0, result);
            Status = $"Rolled {result.Total}";
        }
        catch (Exception ex) when (ex is FormatException or InvalidOperationException)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private void AddTrack()
    {
        Campaign.Tracks.Add(new Track { Name = "Corruption", Description = "A configurable campaign track", Maximum = 10 });
        Status = "Track added";
    }

    [RelayCommand]
    private void Search()
    {
        var count = _storage.Search(Campaign, SearchText).Count();
        Status = $"{count} matching record(s)";
    }

    private void RefreshRecords()
    {
        VisibleRecords.Clear();
        foreach (var record in Campaign.Records.Where(r => r.ModuleId == SelectedModule.Id)) VisibleRecords.Add(record);
    }
}
