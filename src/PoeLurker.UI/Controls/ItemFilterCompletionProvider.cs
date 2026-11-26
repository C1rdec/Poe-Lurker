using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace PoeLurker.UI.Controls;

using System.IO;
public class ItemFilterCompletionProvider : ICompletionProvider
{
    private static readonly Regex ConditionRegex = new(@"^\s*(?<key>Class|BaseType)\b(?<rest>.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex WordRegex = new(@"^\s*(?<word>[A-Za-z]*)$", RegexOptions.Compiled);
    private static readonly Regex BlockRegex = new(@"^(?<word>[A-Za-z]*)$", RegexOptions.Compiled);

    private static readonly string[] Blocks = { "Show", "Hide", "Minimal", "Continue", "Import" };

    private static readonly string[] Conditions =
    {
        "AlternateQuality", "AlwaysShow", "AnyEnchantment", "ArchnemesisMod", "AreaLevel", "BaseArmour",
        "BaseDefencePercentile", "BaseEnergyShield", "BaseEvasion", "BaseType", "BaseWard", "BlightedMap",
        "Class", "Corrupted", "CorruptedMods", "DropLevel", "ElderItem", "ElderMap", "EnchantmentPassiveNode",
        "EnchantmentPassiveNum", "Exceptional", "Fated", "Foulborn", "FracturedItem", "GemLevel",
        "HasCruciblePassiveTree", "HasEaterOfWorldsImplicit", "HasEnchantment", "HasExplicitMod", "HasImplicitMod",
        "HasInfluence", "HasSearingExarchImplicit", "HasVaalUniqueMod", "Height", "Identified", "Imbued",
        "IsVaalUnique", "ItemLevel", "LinkedSockets", "MapTier", "MemoryStrands", "MirageMap", "Mirrored",
        "Quality", "Rarity", "Replica", "Scourged", "ShapedMap", "ShaperItem", "SocketGroup", "Sockets",
        "StackSize", "SynthesisedItem", "TransfiguredGem", "TwiceCorrupted", "UberBlightedMap",
        "UnidentifiedItemTier", "Vestigial", "WaystoneTier", "Width", "ZanaMemory",
    };

    private static readonly HashSet<string> ColorActions = new() { "SetTextColor", "SetBorderColor", "SetBackgroundColor" };

    private static readonly string[] Actions =
    {
        "CustomAlertSound", "CustomAlertSoundOptional", "DisableDropSound", "DisableDropSoundIfAlertSound",
        "EnableDropSound", "EnableDropSoundIfAlertSound", "MinimapIcon", "PlayAlertSound", "PlayAlertSoundPositional",
        "PlayEffect", "SetBackgroundColor", "SetBorderColor", "SetFontSize", "SetTextColor",
    };

    private static readonly Lazy<IReadOnlyList<string>> Classes = new(() => ReadLines("ItemClasses.txt"));
    private static readonly Lazy<IReadOnlyList<string>> BaseTypes = new(() => ReadLines("BaseTypes.txt"));

    public IReadOnlyList<ICompletionData> GetCompletions(string textBeforeCaret, bool forced, out int prefixLength)
    {
        prefixLength = 0;
        var condition = ConditionRegex.Match(textBeforeCaret);
        if (condition.Success)
        {
            return GetValues(condition, forced, out prefixLength);
        }

        var block = BlockRegex.Match(textBeforeCaret);
        if (block.Success)
        {
            if (block.Groups["word"].Length == 0 && !forced)
            {
                return Array.Empty<ICompletionData>();
            }

            prefixLength = block.Groups["word"].Length;
            return Blocks.Select(b => (ICompletionData)new FilterCompletionData(b, b, null, 2)).ToList();
        }

        var word = WordRegex.Match(textBeforeCaret);
        if (!word.Success || (word.Groups["word"].Length == 0 && !forced))
        {
            return Array.Empty<ICompletionData>();
        }

        prefixLength = word.Groups["word"].Length;
        return Conditions.Select(k => (ICompletionData)new FilterCompletionData(k, k + " ", null, 1, "Condition"))
            .Concat(Actions.Select(k => (ICompletionData)new FilterCompletionData(k, ColorActions.Contains(k) ? k + " 255 0 0" : k + " ", null, 0, "Action")))
            .ToList();
    }

    private static IReadOnlyList<ICompletionData> GetValues(Match condition, bool forced, out int prefixLength)
    {
        var rest = condition.Groups["rest"].Value;
        var insideQuote = rest.Count(c => c == '"') % 2 == 1;
        string prefix;

        if (insideQuote)
        {
            prefix = rest[(rest.LastIndexOf('"') + 1)..];
        }
        else
        {
            var lastSeparator = rest.LastIndexOfAny(new[] { ' ', '\t', '=', '<', '>', '!' });
            prefix = lastSeparator < 0 ? rest : rest[(lastSeparator + 1)..];
            if (prefix.Length == 0 && !forced)
            {
                prefixLength = 0;
                return Array.Empty<ICompletionData>();
            }
        }

        prefixLength = prefix.Length;
        var source = string.Equals(condition.Groups["key"].Value, "Class", StringComparison.OrdinalIgnoreCase) ? Classes.Value : BaseTypes.Value;
        return source.Select(v => (ICompletionData)new FilterCompletionData(v, insideQuote ? v : $"\"{v}\"", insideQuote ? "\"" : null, 0)).ToList();
    }

    private static IReadOnlyList<string> ReadLines(string fileName)
    {
        var assembly = typeof(ItemFilterCompletionProvider).Assembly;
        var resource = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
        if (resource == null)
        {
            return Array.Empty<string>();
        }

        using var stream = assembly.GetManifestResourceStream(resource);
        using var reader = new StreamReader(stream);
        var lines = new List<string>();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                lines.Add(line.Trim());
            }
        }

        return lines;
    }

    private class FilterCompletionData : ICompletionData
    {
        private readonly string _insertText;
        private readonly string _closing;

        public FilterCompletionData(string text, string insertText, string closing, double priority, string description = null)
        {
            Text = text;
            _insertText = insertText;
            _closing = closing;
            Priority = priority;
            Description = description;
        }

        public string Text { get; }

        public object Content => new System.Windows.Controls.TextBlock
        {
            Text = Text,
            Foreground = System.Windows.Media.Brushes.White,
            Padding = new System.Windows.Thickness(4, 2, 4, 2),
        };

        public object Description { get; }

        public ImageSource Image => null;

        public double Priority { get; }

        public void Complete(TextArea textArea, ISegment segment, EventArgs insertionRequestEventArgs)
        {
            var document = textArea.Document;
            var insert = _insertText;
            if (_closing != null && (segment.EndOffset >= document.TextLength || document.GetCharAt(segment.EndOffset) != '"'))
            {
                insert += _closing;
            }

            document.Replace(segment, insert);
        }
    }
}
