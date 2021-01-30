//-----------------------------------------------------------------------
// <copyright file="BuildConfigurationView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for BuildConfigurationView.xaml.
    /// </summary>
    public partial class BuildConfigurationView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfigurationView"/> class.
        /// </summary>
        public BuildConfigurationView()
        {
            this.InitializeComponent();
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