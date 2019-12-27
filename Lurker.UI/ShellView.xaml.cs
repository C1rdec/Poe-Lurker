//-----------------------------------------------------------------------
// <copyright file="ShellView.xaml.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI
{
    using System.Windows;
    using System.Windows.Controls;

    public partial class ShellView : Window
    {
        public ShellView()
        {
            this.InitializeComponent();
            this.MainGrid.MouseLeftButtonDown += this.MainGrid_MouseLeftButtonDown;
        }

        private void MainGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
