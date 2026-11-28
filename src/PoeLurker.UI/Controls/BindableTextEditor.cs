using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace PoeLurker.UI.Controls;

public class BindableTextEditor : TextEditor
{
    public static readonly DependencyProperty BoundTextProperty = DependencyProperty.Register(
        nameof(BoundText),
        typeof(string),
        typeof(BindableTextEditor),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundTextChanged));

    public static readonly DependencyProperty CompletionProviderProperty = DependencyProperty.Register(
        nameof(CompletionProvider),
        typeof(ICompletionProvider),
        typeof(BindableTextEditor),
        new PropertyMetadata(null));

    private CompletionWindow _completionWindow;

    public BindableTextEditor()
    {
        Options.ConvertTabsToSpaces = true;
        Options.IndentationSize = 4;
        TextArea.TextView.ElementGenerators.Add(new ColorSwatchGenerator());
        TextArea.TextEntered += OnTextEntered;
        TextArea.PreviewKeyDown += OnPreviewKeyDown;
        IsVisibleChanged += OnIsVisibleChanged;
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            Dispatcher.BeginInvoke(() =>
            {
                TextArea.Focus();
                Keyboard.Focus(TextArea);
                CaretOffset = Document.TextLength;
            }, System.Windows.Threading.DispatcherPriority.Input);
        }
    }

    public string BoundText
    {
        get => (string)GetValue(BoundTextProperty);
        set => SetValue(BoundTextProperty, value);
    }

    public ICompletionProvider CompletionProvider
    {
        get => (ICompletionProvider)GetValue(CompletionProviderProperty);
        set => SetValue(CompletionProviderProperty, value);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        if (BoundText != Text)
        {
            SetCurrentValue(BoundTextProperty, Text);
        }
    }

    private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (BindableTextEditor)d;
        var newText = e.NewValue as string ?? string.Empty;
        if (editor.Text != newText)
        {
            editor.Text = newText;
        }
    }

    private static bool IsBlockRoot(string text)
    {
        var trimmed = text.Trim();
        var end = trimmed.IndexOf('#');
        if (end >= 0)
        {
            trimmed = trimmed.Substring(0, end).Trim();
        }

        return trimmed is "Show" or "Hide" or "Minimal";
    }

    private void OnTextEntered(object sender, TextCompositionEventArgs e)
    {
        if (e.Text == "\n" || e.Text == "\r\n" || e.Text == "\r")
        {
            var current = Document.GetLineByOffset(CaretOffset);
            var previous = current.PreviousLine;
            if (previous != null && IsBlockRoot(Document.GetText(previous.Offset, previous.Length)))
            {
                var indent = new string(' ', Options.IndentationSize);
                Document.Insert(CaretOffset, indent);
            }

            return;
        }

        if (e.Text.Length == 0 || !(char.IsLetterOrDigit(e.Text[0]) || e.Text[0] == '"' || e.Text[0] == ' '))
        {
            return;
        }

        ShowCompletion(false);
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space && Keyboard.Modifiers == ModifierKeys.Control)
        {
            e.Handled = true;
            ShowCompletion(true);
        }
    }

    private static void ApplyCompletionTheme(CompletionWindow window)
    {
        var background = new SolidColorBrush(Color.FromRgb(0x2e, 0x26, 0x14));
        var foreground = Brushes.White;
        var accent = new SolidColorBrush(Color.FromRgb(0xaf, 0x84, 0x04));
        var hover = new SolidColorBrush(Color.FromRgb(0x4a, 0x3c, 0x1c));

        window.Background = background;
        window.Foreground = foreground;
        window.BorderBrush = accent;
        window.BorderThickness = new Thickness(0);
        window.Padding = new Thickness(0);
        window.CompletionList.BorderThickness = new Thickness(0);
        window.CompletionList.Padding = new Thickness(0);
        window.CompletionList.Background = background;
        window.CompletionList.Foreground = foreground;

        var listBox = window.CompletionList.ListBox;
        listBox.Background = background;
        listBox.Foreground = foreground;
        listBox.BorderThickness = new Thickness(0);
        listBox.Margin = new Thickness(0);
        listBox.Padding = new Thickness(0);

        var itemStyle = new Style(typeof(ListBoxItem));
        itemStyle.Setters.Add(new Setter(Control.ForegroundProperty, foreground));
        itemStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0)));
        itemStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0)));
        itemStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
        itemStyle.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.Transparent));
        var hoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        hoverTrigger.Setters.Add(new Setter(Control.BackgroundProperty, hover));
        var selectedTrigger = new Trigger { Property = ListBoxItem.IsSelectedProperty, Value = true };
        selectedTrigger.Setters.Add(new Setter(Control.BackgroundProperty, accent));
        selectedTrigger.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.White));
        itemStyle.Triggers.Add(hoverTrigger);
        itemStyle.Triggers.Add(selectedTrigger);
        itemStyle.Setters.Add(new Setter(Control.TemplateProperty, CreateItemTemplate()));
        listBox.ItemContainerStyle = itemStyle;

        window.Loaded += (_, _) => PaintBackgrounds(window, background, window.CompletionList.ListBox);
    }

    private static void PaintBackgrounds(DependencyObject root, Brush background, ListBox listBox)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is ListBoxItem or System.Windows.Controls.Primitives.ScrollBar)
            {
                continue;
            }

            switch (child)
            {
                case Border border when border.Background != null:
                    border.Background = background;
                    break;
                case Panel panel when panel.Background != null:
                    panel.Background = background;
                    break;
                case ScrollViewer scrollViewer:
                    scrollViewer.Background = background;
                    break;
            }

            PaintBackgrounds(child, background, listBox);
        }
    }

    private static ControlTemplate CreateItemTemplate()
    {
        var border = new FrameworkElementFactory(typeof(Border));
        border.SetBinding(Border.BackgroundProperty, new System.Windows.Data.Binding("Background") { RelativeSource = System.Windows.Data.RelativeSource.TemplatedParent });
        var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
        presenter.SetValue(ContentPresenter.MarginProperty, new Thickness(2));
        border.AppendChild(presenter);
        return new ControlTemplate(typeof(ListBoxItem)) { VisualTree = border };
    }

    private void ShowCompletion(bool forced)
    {
        var provider = CompletionProvider;
        if (provider == null || _completionWindow != null)
        {
            return;
        }

        var line = Document.GetLineByOffset(CaretOffset);
        var textBeforeCaret = Document.GetText(line.Offset, CaretOffset - line.Offset);
        var items = provider.GetCompletions(textBeforeCaret, forced, out var prefixLength);
        if (items.Count == 0)
        {
            return;
        }

        _completionWindow = new CompletionWindow(TextArea)
        {
            StartOffset = CaretOffset - prefixLength,
            CloseWhenCaretAtBeginning = prefixLength > 0,
        };

        foreach (var item in items)
        {
            _completionWindow.CompletionList.CompletionData.Add(item);
        }

        ApplyCompletionTheme(_completionWindow);

        _completionWindow.Closed += (_, _) => _completionWindow = null;
        _completionWindow.Show();
        if (prefixLength > 0)
        {
            _completionWindow.CompletionList.SelectItem(Document.GetText(CaretOffset - prefixLength, prefixLength));
        }
    }
}
