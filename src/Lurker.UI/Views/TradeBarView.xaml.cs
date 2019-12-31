//-----------------------------------------------------------------------
// <copyright file="TradeBarView.xaml.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for TradeBarView.xaml
    /// </summary>
    public partial class TradeBarView : Window
    {
        public TradeBarView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the PreviewMouseWheel event of the ScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
