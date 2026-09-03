using DSC.Toolkit.Core.Models;
using DSC.Toolkit.Core.Services;
using Xunit;

namespace DSC.Toolkit.Tests;

public sealed class CoreTests
{
    [Theory]
    [InlineData("1d20+5", 1, 20, 5)]
    [InlineData("2d6-3", 2, 6, -3)]
    [InlineData("1d100", 1, 100, 0)]
    public void ParsesSafeDiceExpressions(string text, int count, int sides, int modifier)
    {
        var result = DiceExpression.Parse(text);
        Assert.Equal((count, sides, modifier), (result.Count, result.Sides, result.Modifier));
    }

    [Theory]
    [InlineData("0d20")]
    [InlineData("51d6")]
    [InlineData("1d7")]
    [InlineData("System.IO.File.Delete('*')")]
    public void RejectsInvalidDiceExpressions(string text) => Assert.Throws<FormatException>(() => DiceExpression.Parse(text));

    [Fact]
    public void RollsStayWithinRange()
    {
        var roll = new DiceService().Roll("50d4-10");
        Assert.Equal(50, roll.Results.Count);
        Assert.All(roll.Results, value => Assert.InRange(value, 1, 4));
        Assert.Equal(roll.Results.Sum() - 10, roll.Total);
    }

    [Fact]
    public void AdvantageUsesOneD20AndRetainsDiscardedRoll()
    {
        var roll = new DiceService().Roll("1d20+2", mode: RollMode.Advantage);
        Assert.NotNull(roll.Discarded);
        Assert.True(roll.Results[0] >= roll.Discarded![0]);
    }

    [Fact]
    public void CatalogContainsExactlySevenDistinctModules()
    {
        Assert.Equal(7, ModuleCatalog.All.Count);
        Assert.Equal(7, ModuleCatalog.All.Select(module => module.Id).Distinct().Count());
    }

    [Fact]
    public void CanonicalLocksMatchTheManual()
    {
        Assert.Collection(CanonicalLocks.All,
            item => Assert.Equal(("Luminaris", "Luminar", "Shadowmourne"), (item.World, item.Guardian, item.Sword)),
            item => Assert.Equal(("Umbraria", "Umbrael", "Ashbringer"), (item.World, item.Guardian, item.Sword)),
            item => Assert.Equal(("Eternum", "Eternus", "Plaguebringer"), (item.World, item.Guardian, item.Sword)),
            item => Assert.Equal(("Solara", "Solara", "Scourgeborne"), (item.World, item.Guardian, item.Sword)),
            item => Assert.Equal(("Aetheria", "Aetherion", "Thundersoul"), (item.World, item.Guardian, item.Sword)),
            item => Assert.Equal(("Harmonia", "Harmonia", "Chaosbringer"), (item.World, item.Guardian, item.Sword)),
            item => Assert.Equal(("Celestia", "Celestian", "Felmourne"), (item.World, item.Guardian, item.Sword)));
    }

    [Fact]
    public void TrackClampsAndUndoRestoresPreviousValue()
    {
        var track = new Track { Current = 2, Minimum = 0, Maximum = 5 };
        track.Change(20, "test");
        Assert.Equal(5, track.Current);
        Assert.True(track.Undo());
        Assert.Equal(2, track.Current);
    }

    [Fact]
    public async Task CampaignRoundTripsAndImportGetsNewIdentity()
    {
        var folder = Path.Combine(Path.GetTempPath(), $"dsc-{Guid.NewGuid():N}");
        try
        {
            var storage = new CampaignStorage(folder);
            var campaign = new Campaign { Name = "Eternum" };
            campaign.Records.Add(new ToolRecord { Name = "Nyra", Fields = new() { ["Present Temptation"] = "Healing through decay" } });
            await storage.SaveAsync(campaign);
            var loaded = await storage.LoadAsync(storage.GetPath(campaign.Id));
            Assert.Equal("Nyra", loaded.Records.Single().Name);
            var imported = await storage.ImportAsync(storage.GetPath(campaign.Id));
            Assert.NotEqual(campaign.Id, imported.Id);
        }
        finally
        {
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void SearchFindsFieldsAndTags()
    {
        var campaign = new Campaign();
        campaign.Records.Add(new ToolRecord { Name = "Nyra", Tags = ["Eternum"], Fields = new() { ["Compass"] = "violent response" } });
        var storage = new CampaignStorage("unused");
        Assert.Single(storage.Search(campaign, "violent"));
        Assert.Single(storage.Search(campaign, "Eternum"));
    }
}
