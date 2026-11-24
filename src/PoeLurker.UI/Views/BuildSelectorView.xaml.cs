//-----------------------------------------------------------------------
// <copyright file="BuildSelectorView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Views;

using System.Windows.Controls;
using System.Windows.Input;
using PoeLurker.UI.ViewModels;

/// <summary>
/// Interaction logic for BuildSelectorView.xaml.
/// </summary>
public partial class BuildSelectorView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildSelectorView"/> class.
    /// </summary>
    public BuildSelectorView()
    {
        InitializeComponent();
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        var scrollViewer = (ScrollViewer)sender;
        if (scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset <= 50 && DataContext is BuildSelectorViewModel viewModel)
        {
            viewModel.LoadMore();
        }
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)    {
        var scrollViewer = (ScrollViewer)sender;
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        e.Handled = true;
    }
}