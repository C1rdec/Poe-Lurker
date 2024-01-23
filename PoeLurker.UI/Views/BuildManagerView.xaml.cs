//-----------------------------------------------------------------------
// <copyright file="BuildManagerView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Views
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for BuildManagerView.xaml.
    /// </summary>
    public partial class BuildManagerView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildManagerView"/> class.
        /// </summary>
        public BuildManagerView()
        {
            this.InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}