//-----------------------------------------------------------------------
// <copyright file="UpdateWindowView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Views;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using PoeLurker.UI.ViewModels;

/// <summary>
/// Interaction logic for UpdateWindowView.xaml.
/// </summary>
public partial class UpdateWindowView : MetroWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWindowView"/> class.
    /// </summary>
    public UpdateWindowView()
    {
        InitializeComponent();

        if (Content is Grid grid && grid.Children.Count > 0 && grid.Children[0] is StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is TextBlock title && title.FontSize >= 20)
                {
                    title.SetBinding(TextBlock.TextProperty, new Binding(nameof(UpdateWindowViewModel.Title)));
                }
                else if (child is ProgressBar progressBar)
                {
                    _progressBar = progressBar;
                }
            }
        }

        DataContextChanged += OnDataContextChanged;
    }

    private static readonly Brush SuccessBrush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));

    private ProgressBar _progressBar;

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyPropertyChanged oldVm)
        {
            oldVm.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is INotifyPropertyChanged newVm)
        {
            newVm.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UpdateWindowViewModel.IsUpToDate)
            && sender is UpdateWindowViewModel { IsUpToDate: true }
            && _progressBar != null)
        {
            Dispatcher.Invoke(() => _progressBar.Foreground = SuccessBrush);
        }
    }
}
