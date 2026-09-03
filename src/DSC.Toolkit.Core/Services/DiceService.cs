using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DSC.Toolkit.Core.Models;

namespace DSC.Toolkit.Core.Services;

public enum RollMode { Normal, Advantage, Disadvantage }

public sealed record DiceExpression(int Count, int Sides, int Modifier)
{
    private static readonly Regex Pattern = new(@"^\s*(\d{1,2})d(4|6|8|10|12|20|100)\s*([+-]\s*\d{1,4})?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static DiceExpression Parse(string value)
    {
        var match = Pattern.Match(value ?? string.Empty);
        if (!match.Success) throw new FormatException("Use an expression such as 1d20+5 or 2d6-1.");
        var count = int.Parse(match.Groups[1].Value);
        if (count is < 1 or > 50) throw new FormatException("Dice quantity must be between 1 and 50.");
        var modifier = match.Groups[3].Success ? int.Parse(match.Groups[3].Value.Replace(" ", "")) : 0;
        return new(count, int.Parse(match.Groups[2].Value), modifier);
    }
}

public sealed record DiceRoll(DateTimeOffset Timestamp, string Expression, string Label, RollMode Mode, IReadOnlyList<int> Results, int Modifier, int Total, IReadOnlyList<int>? Discarded = null);

public interface IDiceService { DiceRoll Roll(string expression, string? label = null, RollMode mode = RollMode.Normal); }

public sealed class DiceService : IDiceService
{
    public DiceRoll Roll(string expression, string? label = null, RollMode mode = RollMode.Normal)
    {
        var parsed = DiceExpression.Parse(expression);
        if (mode != RollMode.Normal && (parsed.Count != 1 || parsed.Sides != 20))
            throw new InvalidOperationException("Advantage and disadvantage require exactly 1d20.");

        var first = RollSet(parsed);
        IReadOnlyList<int>? discarded = null;
        if (mode != RollMode.Normal)
        {
            var second = RollSet(parsed);
            var keepFirst = mode == RollMode.Advantage ? first.Sum() >= second.Sum() : first.Sum() <= second.Sum();
            discarded = keepFirst ? second : first;
            first = keepFirst ? first : second;
        }
        return new(DateTimeOffset.UtcNow, expression, label?.Trim() ?? "", mode, first, parsed.Modifier, first.Sum() + parsed.Modifier, discarded);
    }

    private static List<int> RollSet(DiceExpression expression) =>
        Enumerable.Range(0, expression.Count).Select(_ => RandomNumberGenerator.GetInt32(1, expression.Sides + 1)).ToList();
}
