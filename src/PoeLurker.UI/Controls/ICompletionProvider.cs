using ICSharpCode.AvalonEdit.CodeCompletion;

namespace PoeLurker.UI.Controls;

public interface ICompletionProvider
{
    IReadOnlyList<ICompletionData> GetCompletions(string textBeforeCaret, bool forced, out int prefixLength);
}
