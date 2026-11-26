using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ColorPicker;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PoeLurker.UI.Controls;

public class ColorSwatchGenerator : VisualLineElementGenerator
{
    private static readonly Regex ColorRegex = new(
        @"^\s*Set(?:Text|Border|Background)Color\s+(?<r>\d{1,3})\s+(?<g>\d{1,3})\s+(?<b>\d{1,3})(?<a>\s+\d{1,3})?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override int GetFirstInterestedOffset(int startOffset)
    {
        var line = CurrentContext.VisualLine.FirstDocumentLine;
        var match = ColorRegex.Match(CurrentContext.Document.GetText(line));
        if (!match.Success)
        {
            return -1;
        }

        var offset = line.Offset + match.Groups["r"].Index;
        return offset >= startOffset ? offset : -1;
    }

    public override VisualLineElement ConstructElement(int offset)
    {
        var document = CurrentContext.Document;
        var line = document.GetLineByOffset(offset);
        var match = ColorRegex.Match(document.GetText(line));
        if (!match.Success)
        {
            return null;
        }

        var picker = new PortableColorPicker
        {
            Width = 14,
            Height = 14,
            Margin = new Thickness(0, 0, 6, 0),
            Cursor = System.Windows.Input.Cursors.Hand,
            ShowAlpha = false,
            ShowFractionalPart = false,
            SelectedColor = Color.FromRgb(ToByte(match.Groups["r"].Value), ToByte(match.Groups["g"].Value), ToByte(match.Groups["b"].Value)),
        };

        if (Application.Current.TryFindResource("DefaultColorPickerStyle") is Style style)
        {
            picker.Style = style;
        }

        var lineOffset = line.Offset;
        var descriptor = DependencyPropertyDescriptor.FromProperty(PortableColorPicker.SelectedColorProperty, typeof(PortableColorPicker));
        descriptor.AddValueChanged(picker, (_, _) => ApplyColor(document, lineOffset, picker.SelectedColor));

        return new InlineObjectElement(0, picker);
    }

    private static void ApplyColor(TextDocument document, int lineOffset, Color color)
    {
        if (lineOffset > document.TextLength)
        {
            return;
        }

        var line = document.GetLineByOffset(lineOffset);
        var match = ColorRegex.Match(document.GetText(line));
        if (!match.Success)
        {
            return;
        }

        var start = line.Offset + match.Groups["r"].Index;
        var end = line.Offset + match.Groups["b"].Index + match.Groups["b"].Length;
        var current = document.GetText(start, end - start);
        var replacement = $"{color.R} {color.G} {color.B}";
        if (current != replacement)
        {
            document.Replace(start, end - start, replacement);
        }
    }

    private static byte ToByte(string value) => (byte)Math.Min(255, int.Parse(value));
}
