//-----------------------------------------------------------------------
// <copyright file="BuildSelectorView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Views;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
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
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Dispatcher.BeginInvoke(
            DispatcherPriority.ContextIdle,
            new Action(() =>
            {
                if (DataContext is not BuildSelectorViewModel viewModel)
                {
                    return;
                }

                var selected = viewModel.Builds.FirstOrDefault(b => b.Selected);
                if (selected == null)
                {
                    return;
                }

                if (Builds.ItemContainerGenerator.ContainerFromItem(selected) is not FrameworkElement container)
                {
                    return;
                }

                var scrollViewer = Builds.Parent as ScrollViewer;
                if (scrollViewer == null)
                {
                    container.BringIntoView();
                    return;
                }

                var top = container.TransformToAncestor(Builds).Transform(new Point(0, 0)).Y;
                var offset = top - ((scrollViewer.ViewportHeight - container.ActualHeight) / 2);
                scrollViewer.ScrollToVerticalOffset(Math.Max(0, offset));
            }));
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